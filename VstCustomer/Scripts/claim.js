$(document).ready(function () {
    $('#lservice').addClass('active');
    $('#month')
     .datepicker({
         format: 'yyyy-mm',
         viewMode: 'months'
     }).on('changeDate', function (ev) {
         $(this).datepicker('hide');
     });
    $('#txtOrderDate,#txtDate,#txtFinish,#txtVisitDate')
    .datepicker({
        format: 'yyyy-mm-dd'
    }).on('changeDate', function (ev) {
        $(this).datepicker('hide');
    });
    //$('#selCus').selectize();
    $('#btnRegOrder').click(function () {
        $('#frmRegOrder').attr('data-id', '');
        $('#frmRegOrder').attr('data-action', 0);
        $('#frmRegOrder')[0].reset();
        $('#txtEmpId').val($('#username').val());
        $('#txtEmpName').val($('#username').attr('data-name'));
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
                  url: $('#hdUrl').val().replace("Action", "GetClaim"),
                  data: function (d) {
                      // note: d is created by datatable, the structure of d is the same with DataTableParameters model above

                      return JSON.stringify({
                          dataTableParameters: d,
                          month: $('#month').val(),
                          cust_id: $('#selCus').val(),
                          status: $('#selStatus').val(),
                      });
                  }
              }

          });
    $('#btnDel').click(function () {
        if ($("input:checkbox:checked").length > 0) {
            var id = $("#tbMainDefault tr").find("input[type='checkbox']:checked").attr('id');
            var em = $("#tbMainDefault tr").find("input[type='checkbox']:checked").attr('data-emp');
            var role = $('#username').attr('data-role');
            var emp = $('#username').val();
            if (em != emp && role < 1) {
                bootbox.alert('You cannot delete this claim');
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
    $("#btnModify").click(function () {
        if ($("input:checkbox:checked").length > 0) {
            $("h4").html("<span class='glyphicon glyphicon-edit'></span> MODIFY CLAIM");
            var id = $("#tbMainDefault tr").find("input[type='checkbox']:checked").attr('id');
            var cus_id = $("#tbMainDefault tr").find("input[type='checkbox']:checked").attr('data-cus');
            var emp_id = $("#tbMainDefault tr").find("input[type='checkbox']:checked").attr('data-emp');
            $('#frmRegOrder').attr('data-id', id);
            $('#frmRegOrder').attr('data-action', 1);
            $('#frmRegOrder')[0].reset();
            if (id) {
                $.ajax({
                    url: $('#hdUrl').val().replace("Action", "GetClaimById"),
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
                                var CLAIM_DATE = data[i].CLAIM_DATE == null ? "" : moment(data[i].CLAIM_DATE).format('YYYY-MM-DD');
                                var VISIT_DATE = data[i].VISIT_DATE == null ? "" : moment(data[i].VISIT_DATE).format('YYYY-MM-DD');
                                var FINISH_DATE = data[i].FINISH_DATE == null ? "" : moment(data[i].FINISH_DATE).format('YYYY-MM-DD');
                                $('#txtEmpId').val(emp_id);
                                $('#txtCoilNo').val(data[i].COIL_NO);
                                $('#txtDate').val(CLAIM_DATE);
                                $('#txtClmWgt').val(data[i].CLAIM_WGT);
                                $('#txtNetWgt').val(data[i].NET_WGT);
                                $('#txtVisitDate').val(VISIT_DATE);
                                $('#txtDefectCd').val(data[i].DEFECT_CD);
                                $('#selDefectLn').val($.trim(data[i].DEFECT_LINE));
                                $('#selStt').val($.trim(data[i].STATUS));
                                $('#txtFinish').val(FINISH_DATE);
                                $('#txtCompensation').val(data[i].COMPENT);
                                $('#txtEndUser').val($.trim(data[i].END_USER));
                                $('#txtCusId').val(data[i].CUSTOMER_ID);
                                $('#txtContent').val(data[i].REMARK);
                                $('#txtCusId').val(cus_id);
                                $('#txtCusId').change();
                                $('#txtSpec').val(data[i].SPEC);
                                $('#txtThk').val(data[i].COIL_THK);
                                $('#txtWth').val(data[i].COIL_WTH);
                                $('#txtGrade').val($.trim(data[i].GRADE));
                                $('#txtSurface').val($.trim(data[i].SURFACE_CD));
                                $('#txtDeffKind').val($.trim(data[i].DEFFECT_KIND));
                                $('#upload').attr('data-path', $.trim(data[i].ATTACHMENT));
                                $('#selGroup').val($.trim(data[i].TYPE));
                                if (data[i].ATTACHMENT) {
                                    $('.upload a').remove();

                                    var href = $('#hdUrl').val().replace("Action", "Download") + "?file=" + data.result;

                                    var a = '<a id="download1" href="' + href + '")">Download</a>';
                                    $('.upload').append(a);
                                }
                                $('#txtCusId,#txtEndUser').change();
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
   
    $('#frmRegOrder').submit(function (e) {
        var em = $.trim($('#frmRegOrder').attr('data-em'));
        var role = $('#username').attr('data-role');
        var emp = $('#username').attr('data-emp');
        e.preventDefault();
        //if ($('#frmRegOrder').smkValidate()) {
        var id = $.trim($('#frmRegOrder').attr('data-id'));
        var action = $('#frmRegOrder').attr('data-action');
        var claim = {
            CLAIM_NO: id,
            EMP_ID: $.trim($('#txtEmpId').val()),
            CLAIM_DATE: $.trim($('#txtDate').val()),
            CUSTOMER_ID: $.trim($('#txtCusId').val()),
            END_USER: $.trim($('#txtEndUser').val()),
            COIL_NO: $.trim($('#txtCoilNo').val()),
            CLAIM_WGT: $.trim($('#txtClmWgt').val()),
            NET_WGT: $.trim($('#txtNetWgt').val()),
            VISIT_DATE: $.trim($('#txtVisitDate').val()),
            DEFECT_CD: $.trim($('#txtDefectCd').val()),
            DEFECT_LINE: $.trim($('#selDefectLn').val()),
            FINISH_DATE: $.trim($('#txtFinish').val()),
            SPEC: $.trim($('#txtSpec').val()),
            STATUS: $.trim($('#selStt').val()),
            COMPENT: $.trim($('#txtCompensation').val()),
            REMARK: $.trim($('#txtContent').val()),
            STS_ST_CLS: $.trim($('#txtSpec').val()),
            COIL_THK: $.trim($('#txtThk').val()),
            COIL_WTH: $.trim($('#txtWth').val()),
            SURFACE_CD: $.trim($('#txtSurface').val()),
            GRADE: $.trim($('#txtGrade').val()),
            DEFFECT_KIND: $.trim($('#txtDeffKind').val()),
            TYPE: $.trim($('#selGroup').val()),
            ATTACHMENT: $.trim($('#upload').attr('data-path'))
        };

        $.ajax({
            url: $('#hdUrl').val().replace("Action", "InsertUpdateClaim"),
            data: JSON.stringify({
                CLAIM: claim,
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

    $('#btnSearch').click(function () {
        tbClaim.draw();
        return false;
    });
    $('#txtCusId').change(function () {
        $("#cusid option").remove();
        var cus_id = $.trim($(this).val());
        if ($(this).val().length >= 5) {
            $.ajax({
                url: $('#hdUrl1').val().replace("Action", "GetEndUser"),
                data: JSON.stringify({
                    CUS_ID: cus_id
                }),
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                crossBrowser: true,
                success: function (data, status) {
                    var e = data.END;
                    $('#txtEndUser,#txtEndName').val('');
                 
                    if ($.trim(e[0].END_USER_ID) == cus_id) {
                        $('#txtEndUser').val($.trim(cus_id));
                        $('#txtEndUser').change();
                    }
                    $('#txtCusName').val($.trim(data.CUSTOMER.NAME));
                    for (var i = 0; i < e.length; i++) {
                        $('#cusid').append("<option value='" + $.trim(e[i].END_USER_ID) + "'>");

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
    $('#txtEndUser').change(function () {

        var cus_id = $.trim($(this).val());
        if ($(this).val().length >= 5) {
            $.ajax({
                url: $('#hdUrl1').val().replace("Action", "GetEndUserById"),
                data: JSON.stringify({
                    ID: cus_id
                }),
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                crossBrowser: true,
                success: function (data, status) {
                    $('#txtEndName').val($.trim(data.NAME));
                    $('#txtEndUser').val($.trim(cus_id));
                    return false;
                },
                error: function (xhr, status, error) {
                    bootbox.alert("Error! " + xhr.status);
                },
            });
        }
        return false;
    });
    $('#btnExport').click(function () {
        var month = $('#month').val();
        var cus_id = $('#selCus').val();
        var status = $('#selStatus').val();
        var url = $('#hdUrl').val().replace("Action", "Export") + "?month=" + month + "&cus_id=" + cus_id + "&status=" + status;
        window.location.href = url;
        return false;
    });
    $("input:file#upload").fileupload({
        dataType: "json",
        url: $('#hdUrl').val().replace("Action", "Upload"),
        autoUpload: true,
        add: function (e, data) {
            var uploadErrors = [];
            var fileType = data.originalFiles[0].name.split('.').pop(), allowdtypes = 'xls,xlsx';
            if (allowdtypes.indexOf(fileType) < 0) {
                uploadErrors.push('Invalid file type. Only excel file allowed');

            }
            if (data.originalFiles[0]['size'].length && data.originalFiles[0]['size'] > 5000000) {
                uploadErrors.push('Filesize is too big. Maximum is 5 Mb');

            }
            if (uploadErrors.length > 0) {
                alert(uploadErrors.join("\n"));
            } else {
                data.submit();
            }
        },
        //send: function () {
        //    spinner.spin(target);
        //},
        done: function (e, data) {
            var errors = data.result.Errors;
            if (errors && errors.length) {
                alert(errors);
            } else {
                $('#upload').attr('data-path', data.result);
                $('.upload a').remove();

                var href = $('#hdUrl').val().replace("Action", "Download") + "?file=" + data.result;

                var a = '<a id="download1" href="' + href + '")">Download</a>';
                $('.upload').append(a);

                alert("Upload Success");

            }
        },
        always: function () {
            //spinner.spin(false);
        }
    });
});