
$(document).ready(function () {
    $('#loder').addClass('active');
    $('#from,#to')
    .datepicker({
        format: 'yyyy-mm-dd'
    }).on('changeDate', function (ev) {
        $(this).datepicker('hide');
    });
    $('#txtDelivery')
     .datepicker({
         format: 'yyyy-mm',
         viewMode: 'months'
     }).on('changeDate', function (ev) {
         $(this).datepicker('hide');
     });
    $('#txtOrderDate')
    .datepicker({
        format: 'yyyy-mm-dd'
    }).on('changeDate', function (ev) {
        $(this).datepicker('hide');
    });
    $('#btnRegOrder').click(function () {
        $("h4").html("<span class='glyphicon glyphicon-edit'></span> NEW ORDER");
        $('#frmRegOrder')[0].reset();
        $('#txtEmpId').val($('#username').val());
        $('#txtEmpName').val($('#username').attr('data-name'));
        $('#frmRegOrder').attr('data-action', 0);
        $("#mdRegOrder").modal({
            backdrop: 'static',
            keyboard: false
        });
        return false;
    });
    $("#tbMainDefault").on('click', 'tr', function (e) {
        if (!$(this).parent("thead").is('thead')) {
            $("tr").removeClass("success");
            $(this).addClass("success");
            if (!$(e.target).is('#tbMainDefault td input:checkbox')) {
                $(this).find('input:checkbox').trigger('click');
            }
        }
    });
    $("#tbMainDefault").on('change', '.ckb', function () {
        var group = ":checkbox[class='" + $(this).attr("class") + "']";
        if ($(this).is(':checked')) {
            $(group).not($(this)).attr("checked", false);

        }

    });
   
    var tbClaim = $('#tbMainDefault').DataTable(
         {
             sort: false,
             "processing": true,
             "serverSide": true,
             "searching": false,
             ajax: {
                 type: "POST",
                 contentType: "application/json",
                 url: $('#hdUrl').val().replace("Action", "GetOrder"),
                 data: function (d) {
                     // note: d is created by datatable, the structure of d is the same with DataTableParameters model above

                     return JSON.stringify({
                         dataTableParameters: d,
                         from: $('#from').val(),
                         to: $('#to').val(),
                         cust_id: $('#selCus').val(),
                         status: $('#selStatus').val(),
                     });
                 }
             }

         });
    $('#btnDel').click(function () {
        if ($("input:checkbox:checked").length > 0) {
            var id = $("#tbMainDefault tr").find("input[type='checkbox']:checked").attr('id');
            var id = $("#tbMainDefault tr").find("input[type='checkbox']:checked").attr('id');
            var em = $("#tbMainDefault tr").find("input[type='checkbox']:checked").attr('data-emp');
            var role = $('#username').attr('data-role');
            var emp = $('#username').val();
            if (em != emp && role < 1) {
                bootbox.alert('You cannot delete this order');
                return false;
            }
            var cfm = confirm("Are you sure you want to delete this claim?");
            if (cfm) {
                $.ajax({
                    url: $('#hdUrl').val().replace("Action", "Delete"),
                    data: JSON.stringify({
                        ID: id
                    }),
                    type: 'POST',
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',
                    crossBrowser: true,
                    success: function (data, status) {
                        if (data > 0) {
                            tbClaim.ajax.reload();
                            bootbox.alert(status);
                        }
                        else {
                            bootbox.alert('Delete fail');
                        }
                        return false;
                    },
                    error: function (xhr, status, error) {
                        bootbox.alert("Error! " + xhr.status);
                    },
                });
            }
        }
        else {
            bootbox.alert("Please select a row");
        }
        return false;
    });
    $("#btnCopy").click(function () {
        if ($("input:checkbox:checked").length > 0) {
            $('#frmRegOrder')[0].reset();
            $("h4").html("<span class='glyphicon glyphicon-edit'></span> NEW ORDER");
            var id = $("#tbMainDefault tr").find("input[type='checkbox']:checked").attr('id');
            var cus_id = $("#tbMainDefault tr").find("input[type='checkbox']:checked").attr('data-cus');
            var emp_id = $('#username').val();
            $('#frmRegOrder').attr('data-id', '');
            $('#frmRegOrder').attr('data-action', 0);
           
            if (id) {
                $.ajax({
                    url: $('#hdUrl').val().replace("Action", "GetOrderById"),
                    data: JSON.stringify({
                        ID: id
                    }),
                    type: 'POST',
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',
                    crossBrowser: true,
                    success: function (data, status) {
                        if (data.length > 0) {
                            for (var i = 0; i < data.length; i++) {
                                var ORDED_DATE = data[i].ORDED_DATE == null ? "" : moment(data[i].ORDED_DATE).format('YYYY-MM-DD');
                                $('#txtOrderDate').val(ORDED_DATE);
                                $('#txtEmpId').val($.trim(emp_id));
                                $('#selCr').val($.trim(data[i].ORDER_CR_HR));
                                $('#selCustomer').val($.trim(data[i].CUSTOMER_ID));
                                $('#selCustomer').change();
                                $('#selSurface').val($.trim(data[i].SURFACE_CD));
                                $('#selTHK').val(data[i].ORD_THK);
                                $('#selWTH').val(data[i].ORD_WTH);
                                $('#selEdge').val(data[i].ORD_EDGE);
                                //$('#selWGT').val(data[i].ORD_WGT);
                                $('#txtBasePrice').val(data[i].BASE_PRICE);
                                $('#txtEffPrice').val(data[i].EFFECT_PRICE);
                                $('#txtBidPrice').val(data[i].BIDD_PRICE);
                                $('#txtOrderNo').val(data[i].CONTRACT_NO);
                                $('#selUsage').val(data[i].ORD_USAGE);
                                $('#selStt').val($.trim(data[i].ORD_STAT));
                                $('#txtQuantity').val($.trim(data[i].QUANTITY));
                                $('#selEndUser').val($.trim(data[i].END_USER));
                                $('#selGrade').val($.trim(data[i].STS_ST_CLS));
                                $('#selSeries').val($.trim(data[i].STS_ST_SER));
                                $('#selGrade').val($.trim(data[i].STS_ST_CLS));
                                $('#txtDelivery').val($.trim(data[i].DELIVERY_TIME));
                                $('#txtRemark').val($.trim(data[i].REMARK));
                                $('#selEndUser').change();
                            }
                        }

                        return false;
                    },
                    error: function (xhr, status, error) {
                        bootbox.alert("Error! " + xhr.status);
                    },
                });
              
            }
            $("#mdRegOrder").modal({
                backdrop: 'static',
                keyboard: false
            });
        }
        else {
            bootbox.alert("Please select a row.");
        }
        return false;
    });
    $("#btnModify").click(function () {
        if ($("input:checkbox:checked").length > 0) {
            $("h4").html("<span class='glyphicon glyphicon-edit'></span> MODIFY ORDER");
            var id = $("#tbMainDefault tr").find("input[type='checkbox']:checked").attr('id');
            var cus_id = $("#tbMainDefault tr").find("input[type='checkbox']:checked").attr('data-cus');
            var emp_id = $("#tbMainDefault tr").find("input[type='checkbox']:checked").attr('data-emp');
            $('#frmRegOrder').attr('data-id', id);
            $('#frmRegOrder').attr('data-action', 1);
            $('#frmRegOrder')[0].reset();
            if (id) {
                $.ajax({
                    url: $('#hdUrl').val().replace("Action", "GetOrderById"),
                    data: JSON.stringify({
                        ID: id
                    }),
                    type: 'POST',
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',
                    crossBrowser: true,
                    success: function (data, status) {
                        if (data.length > 0) {
                            for (var i = 0; i < data.length; i++) {
                                var ORDED_DATE = data[i].ORDED_DATE == null ? "" : moment(data[i].ORDED_DATE).format('YYYY-MM-DD');
                                $('#txtOrderDate').val(ORDED_DATE);
                                $('#txtEmpId').val($.trim(emp_id));
                                $('#selCr').val($.trim(data[i].ORDER_CR_HR));
                                $('#selCustomer').val($.trim(data[i].CUSTOMER_ID));
                                //$('#selEndUser').val();
                               
                                $('#selCustomer').change();
                                $('#selSurface').val($.trim(data[i].SURFACE_CD));
                                $('#selTHK').val(data[i].ORD_THK);
                                $('#selWTH').val(data[i].ORD_WTH);
                                $('#selEdge').val(data[i].ORD_EDGE);
                                //$('#selWGT').val(data[i].ORD_WGT);
                                $('#txtBasePrice').val(data[i].BASE_PRICE);
                                $('#txtEffPrice').val(data[i].EFFECT_PRICE);
                                $('#txtBidPrice').val(data[i].BIDD_PRICE);
                                $('#txtOrderNo').val(data[i].CONTRACT_NO);
                                $('#selUsage').val(data[i].ORD_USAGE);
                                $('#selStt').val($.trim(data[i].ORD_STAT));
                                $('#txtQuantity').val($.trim(data[i].QUANTITY));
                                $('#selEndUser').val($.trim(data[i].END_USER));
                                $('#selGrade').val($.trim(data[i].STS_ST_CLS));
                                $('#selSeries').val($.trim(data[i].STS_ST_SER));
                                $("#selEndUser option:selected").attr($.trim(data[i].END_USER));
                                $('#txtDelivery').val($.trim(data[i].DELIVERY_TIME));
                                $('#txtRemark').val($.trim(data[i].REMARK));
                                $('#selEndUser').change();
                            }
                        }

                        return false;
                    },
                    error: function (xhr, status, error) {
                        bootbox.alert("Error! " + xhr.status);
                    },
                });
                
            }
            $("#mdRegOrder").modal({
                backdrop: 'static',
                keyboard: false
            });
        }
        else {
            bootbox.alert("Please select a row.");
        }
        return false;
    });
   
    $('#selCustomer').change(function () {
        $("#cus2 option").remove();
        var cus_id =$.trim( $(this).val());
        if ($(this).val().length >= 5) {
            $.ajax({
                url: $('#hdUrl').val().replace("Action", "GetEndUser"),
                data: JSON.stringify({
                    CUS_ID: cus_id
                }),
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                crossBrowser: true,
                success: function (data, status) {
                    var e = data.END;
                    $('#selEndUser,#txtEndUser').val('');
                 
                    if ($.trim(e[0].END_USER_ID) == cus_id) {
                        $('#selEndUser').val($.trim(cus_id));
                        $('#selEndUser').change();
                    }
                   
                    $('#txtCustName').val($.trim(data.CUSTOMER.NAME));
                    for (var i = 0; i < e.length; i++) {
                       
                        $('#cus2').append("<option value='" + $.trim(e[i].END_USER_ID) + "'>");
                      
                    }

                    return false;
                },
                error: function (xhr, status, error) {
                    bootbox.alert("Error! " + xhr.status);
                },
            });
        }
        return false;
    });
    $('#selEndUser').change(function () {
      
        var cus_id = $.trim($(this).val());
        if ($(this).val().length >= 5) {
            $.ajax({
                url: $('#hdUrl').val().replace("Action", "GetEndUserById"),
                data: JSON.stringify({
                    ID: cus_id
                }),
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                crossBrowser: true,
                success: function (data, status) {
                    $('#txtEndUser').val($.trim(data.NAME));
                    $('#selEndUser').val($.trim(cus_id));
                    return false;
                },
                error: function (xhr, status, error) {
                    bootbox.alert("Error! " + xhr.status);
                },
            });
        }
        return false;
    });
    $('#frmRegOrder').submit(function (e) {
        e.preventDefault();
        //if ($('#frmRegOrder').smkValidate()) {
        var id = $.trim($('#frmRegOrder').attr('data-id'));
        var action = $('#frmRegOrder').attr('data-action');

        var order = {
            ID: id,
            ORDED_DATE: $.trim($('#txtOrderDate').val()),
            EMP_ID: $.trim($('#txtEmpId').val()),
            ORDER_CR_HR: $.trim($('#selCr').val()),
            CUSTOMER_ID: $.trim($('#selCustomer').val()),
            SURFACE_CD: $.trim($('#selSurface').val()),
            ORD_THK: $.trim($('#selTHK').val()),
            ORD_WTH: $.trim($('#selWTH').val()),
            ORD_EDGE: $.trim($('#selEdge').val()),
            ORD_WGT: 0, //$.trim($('#selWGT').val()),
            BASE_PRICE: $.trim($('#txtBasePrice').val()),
            EFFECT_PRICE: $.trim($('#txtEffPrice').val()),
            BIDD_PRICE: $.trim($('#txtBidPrice').val()),
            CONTRACT_NO: $.trim($('#txtOrderNo').val()),
            ORD_USAGE: $.trim($('#selUsage').val()),
            ORD_STAT: $.trim($('#selStt').val()),
            STS_ST_CLS: $.trim($('#selGrade').val()),
            STS_ST_SER: $.trim($('#selSeries').val()),
            QUANTITY: $('#txtQuantity').val(),
            END_USER: $('#selEndUser').val(), 
            DELIVERY_TIME: $.trim($('#txtDelivery').val()),
            REMARK: $.trim($('#txtRemark').val())
        };


        $.ajax({
            url: $('#hdUrl').val().replace("Action", "InsertUpdateOrder"),
            data: JSON.stringify({
                ORDER: order,
                ACTION: action
            }),
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            crossBrowser: true,
            success: function (data, status) {
                bootbox.alert(status);
                tbClaim.ajax.reload();
                //$('#mdRegOrder').modal('hide');
                $('#frmRegOrder')[0].reset();
                return false;
            },
            error: function (xhr, status, error) {
                bootbox.alert("Error! " + xhr.status);
            },
        });

        //}
        //else {
        //    bootbox.alert('Invalid field');
        //}

        return false;
    });
    
    $('#btnExport').click(function () {
        var from = $('#from').val();
        var to = $('#to').val();
        var cus_id = $('#selCus').val();
        var status = $('#selStatus').val();
        var url = $('#hdUrl').val().replace("Action", "Export") + "?from=" + from + "&to="+to+"&cus_id=" + cus_id + "&status=" + status;
        window.location.href = url;
        return false;
    });
    $('#btnExportReport').click(function () {
        var from = $('#from').val();
        var to = $('#to').val();
        var cus_id = $('#selCus').val();
        var status = $('#selStatus').val();
        var url = $('#hdUrl').val().replace("Action", "ExportReport") + "?from=" + from + "&to=" + to + "&cus_id=" + cus_id + "&status=" + status;
        window.location.href = url;
        return false;
    });
    $('#btnSearch').click(function () {
        tbClaim.draw();
        return false;
    });
});