$(document).ready(function () {
    $('#lvisit').addClass('active');
    $('#from,#to,#txtTargetDate,#txtDate')
      .datepicker({
          format: 'yyyy-mm-dd'
      }).on('changeDate', function (ev) {
          $(this).datepicker('hide');
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
                url: $('#hdUrl').val().replace("Action", "GetVisit"),
                data: function (d) {
                    // note: d is created by datatable, the structure of d is the same with DataTableParameters model above

                    return JSON.stringify({
                        dataTableParameters: d,
                        from: $('#from').val(),
                        to: $('#to').val(),
                        cus_id: $('#selCus').val(),
                        emp_id: $('#selEmp').val()
                    });
                }
            }

        });
    $('#btnDel').click(function () {
        var role = $('#username').attr('data-role');
        var emp_id = $('#username').val();
        if ($("input:checkbox:checked").length > 0) {
            var id = $("#tbMainDefault tr").find("input[type='checkbox']:checked").attr('id');
            var rec_emp_id = $("#tbMainDefault tr").find("input[type='checkbox']:checked").attr('data-emp');
            if (emp_id != rec_emp_id && role < 1) {
                bootbox.alert('You cannot delete this visit');
                return false;
            }
            var cfm = confirm("Are you sure you want to delete this visit?");
            if (cfm) {
                $.ajax({
                    url: $('#hdUrl1').val().replace("Action", "Delete"),
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
    $("#btnRegOrder").click(function () {
        $('#frmRegVisit')[0].reset();
        $('#frmRegVisit').attr('data-action', 0);
        $('#txtEmpId').val($('#username').val());
        $("h4").html("<span class='glyphicon glyphicon-edit'></span> NEW VISIT");
        $("#mdRegVisit").modal({
            backdrop: 'static',
            keyboard: false
        });

        return false;
    });
    $("#btnCopy").click(function () {
        var emp_id = $('#username').val();
        $('#frmRegVisit')[0].reset();
        if ($("input:checkbox:checked").length > 0) {
            $("h4").html("<span class='glyphicon glyphicon-edit'></span> NEW VISIT");
            var id = $("#tbMainDefault tr").find("input[type='checkbox']:checked").attr('id');
            var cus_id = $("#tbMainDefault tr").find("input[type='checkbox']:checked").attr('data-cus');
          
            $('#frmRegVisit').attr('data-id', '');
            $('#frmRegVisit').attr('data-emp', emp_id);

            $('#frmRegVisit').attr('data-action', 0);

            if (id) {
                $.ajax({
                    url: $('#hdUrl').val().replace("Action", "GetVisitById"),
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
                                var d = data[i].CONTACT_DATE == null ? "" : moment(data[i].CONTACT_DATE).format('YYYY-MM-DD');
                                $('#txtDate').val(d);
                                $('#txtEmpId').val($.trim(emp_id));

                                $('#selCustomer').val($.trim(data[i].CUSTOMER_ID));
                                $('#selCustomer').change();
                                $('#selType').val($.trim(data[i].CUST_VIST_TYPE));
                                $('#selPurpose').val($.trim(data[i].CUST_VIST_PURPOSE));
                                $('#txtRemark').val($.trim(data[i].VIST_REMARK));

                            }
                        }

                        return false;
                    },
                    error: function (xhr, status, error) {
                        bootbox.alert("Error! " + xhr.status);
                    },
                });
                $("#mdRegVisit").modal({
                    backdrop: 'static',
                    keyboard: false
                });
            }
        }
        else {
            bootbox.alert('Please select a row');
        }
        return false;
    });

    $("#btnModify").click(function () {
        $('#frmRegVisit')[0].reset();
        if ($("input:checkbox:checked").length > 0) {
            $("h4").html("<span class='glyphicon glyphicon-edit'></span> MODIFY VISIT");
            var id = $("#tbMainDefault tr").find("input[type='checkbox']:checked").attr('id');
            var cus_id = $("#tbMainDefault tr").find("input[type='checkbox']:checked").attr('data-cus');
            var emp_id = $("#tbMainDefault tr").find("input[type='checkbox']:checked").attr('data-emp');
            $('#frmRegVisit').attr('data-id', id);
            $('#frmRegVisit').attr('data-emp', emp_id);
        
            $('#frmRegVisit').attr('data-action', 1);
          
            if (id) {
                $.ajax({
                    url: $('#hdUrl').val().replace("Action", "GetVisitById"),
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
                                var d = data[i].CONTACT_DATE == null ? "" : moment(data[i].CONTACT_DATE).format('YYYY-MM-DD');
                                $('#txtDate').val(d);
                                $('#txtEmpId').val($.trim(emp_id));
                              
                                $('#selCustomer').val($.trim(data[i].CUSTOMER_ID));
                                $('#selCustomer').change();
                                $('#selType').val($.trim(data[i].CUST_VIST_TYPE));
                                $('#selPurpose').val($.trim(data[i].CUST_VIST_PURPOSE));
                                $('#txtRemark').val($.trim(data[i].VIST_REMARK));
                               
                            }
                        }

                        return false;
                    },
                    error: function (xhr, status, error) {
                        bootbox.alert("Error! " + xhr.status);
                    },
                });
                $("#mdRegVisit").modal({
                    backdrop: 'static',
                    keyboard: false
                });
            }
        }
        else {
            bootbox.alert('Please select a row');
        }
        return false;
    });


    $('#selCustomer').change(function () {
        var id = $(this).val();

        if ($(this).val().length >= 5) {
            $('#selContact option').remove();
            $.ajax({
                url: $('#hdUrl1').val().replace("Action", "GetContact"),
                data: JSON.stringify({
                    CUS_ID: id
                }),
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                crossBrowser: true,
                success: function (data, status) {
                    $('#txtCus').val(data.CUSTOMER.NAME);
                    for (var i = 0; i < data.CONTACTS.length; i++) {
                        $('#selContact').append($('<option>', {
                            value: data.CONTACTS[i].ID,
                            text: data.CONTACTS[i].NAME
                        }));
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
    $('#frmRegVisit').submit(function (e) {
        e.preventDefault();
        var role = $('#username').attr('data-role');
        var emp = $('#username').attr('data-emp');

        //if ($('#frmRegOrder').smkValidate()) {
       
        var visit = {
            ID: $.trim($('#frmRegVisit').attr('data-id')),
            EMP_ID: $.trim($('#txtEmpId').val()),
            CUSTOMER_ID: $.trim($('#selCustomer').val()),
            CUST_CONTACTOR: $.trim($('#selContact').val()),
            CONTACT_DATE: $.trim($('#txtDate').val()),
            CUST_VIST_TYPE: $.trim($('#selType').val()),
            CUST_VIST_PURPOSE: $.trim($('#selPurpose').val()),
            VIST_REMARK: $.trim($('#txtRemark').val())
        };

            $.ajax({
                url: $('#hdUrl1').val().replace("Action", "InsertUpdateVisit"),
                data: JSON.stringify({
                    VISIT: visit,
                    ROLE: role
                }),
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                crossBrowser: true,
                success: function (data, status) {
                    if (data > 0) {
                        bootbox.alert(status);
                        tbClaim.ajax.reload();
                        $("#mdRegVisit").modal('hide');
                    }
                    if (data == -1) {
                        alert('Please select contactor');
                    }
                  
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

    $('#btnSearch').click(function () {
        tbClaim.draw();
        return false;
    });
});