$(document).ready(function () {
    $('#lemployee').addClass('active');
    //$('#selCus').selectize();
    $('#btnRegEmp').click(function () {
        $('#frmRegEmp').attr('data-action', 0);
        $('#frmRegEmp')[0].reset();
        $("#tbCus tbody").empty();
        $("#mdRegCus").modal({
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
    $('#btnReset').click(function () {
        if ($("input:checkbox:checked").length > 0) {
            var id = $("#tbMainDefault tr").find("input[type='checkbox']:checked").attr('id');
           
            if (id) {
                $.ajax({
                    url: $('#hdUrl').val().replace("Action", "ResetPassword"),
                    data: JSON.stringify({
                        ID: id
                    }),
                    type: 'POST',
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',
                    crossBrowser: true,
                    success: function (data, status) {
                        if (data > 0) {
                            tbEmp.ajax.reload();
                            bootbox.alert('Success. Password is 123456');
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
            alert('Please select an employee');
        }
        return false;
    });
    $('#btnDelete').click(function () {
        var cus = [];
        $.each($(".ck"), function () {
            if ($(this).is(':checked')) {
                cus.push($(this).attr('data-id'));
            }
        });
       
        var cfm = confirm('Are you sure you want to remove these customers?');
        if (cfm) {
            $.ajax({
                url: $('#hdUrl').val().replace("Action", "DeleteCustomer"),
                data: JSON.stringify({
                    ID: $('#txtEmpId').val(),
                    CUS_ID: cus
                }),
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                crossBrowser: true,
                success: function (data, status) {
                    if (data > 0) {
                        GetCusByEmpId($('#txtEmpId').val());
                        tbEmp.ajax.reload();

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
        return false;
    });
    $('#btnAdd').click(function () {
        var id = $('#selCus').val();
        var emp_id = $.trim($('#txtEmpId').val());
        $.each($(".cus"), function () {
            if (id == $.trim($(this).text())) {
                bootbox.alert('Customer exists! ' + id);
                return false;
            }
        });
        if (emp_id) {
            $.ajax({
                url: $('#hdUrl').val().replace("Action", "InsertEmpCus"),
                data: JSON.stringify({
                    ID: emp_id,
                    CUS_ID: id,
                    NAME: $('#txtEmpName').val()
                }),
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                crossBrowser: true,
                success: function (data, status) {
                    if (data > 0) {
                        GetCusByEmpId(emp_id);
                        tbEmp.ajax.reload();

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
        else {
            alert('Please enter employee id');
        }
        return false;
    });

    var tbEmp = $('#tbMainDefault').DataTable(
          {
              sort: false,
              "processing": true,
              "serverSide": true,
              "searching": true,
              ajax: {
                  type: "POST",
                  contentType: "application/json",
                  url: $('#hdUrl').val().replace("Action", "GetEmployee"),
                  data: function (d) {
                      // note: d is created by datatable, the structure of d is the same with DataTableParameters model above

                      return JSON.stringify({ dataTableParameters: d });
                  }
              }

          });
    $('#btnDel').click(function () {
        if ($("input:checkbox:checked").length > 0) {
            var id = $("#tbMainDefault tr").find("input[type='checkbox']:checked").attr('id');
            var cfm = confirm("Are you sure you want to delete this employee?");
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
                            tbEmp.ajax.reload();
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
            $("h4").html("<span class='glyphicon glyphicon-edit'></span> MODIFY EMPLOYEE");
            var id = $("#tbMainDefault tr").find("input[type='checkbox']:checked").attr('id');
            $('#frmRegEmp').attr('data-action', 1);
            $('#frmRegEmp')[0].reset();
            $("#tbCus tbody").empty();
            if (id) {
                GetCusByEmpId(id);
               
                $.ajax({
                    url: $('#hdUrl').val().replace("Action", "GetEmployeeById"),
                    data: JSON.stringify({
                        ID: id
                    }),
                    type: 'POST',
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',
                    crossBrowser: true,
                    success: function (data, status) {
                        if (data.length > 0) {
                            $('#txtEmpId').val($.trim(data[0].EMP_ID));
                            $('#txtEmpName').val($.trim(data[0].EMP_NAME));
                            $('#selDept').val($.trim(data[0].EMP_DEPT));
                            $('#txtEmail').val($.trim(data[0].EMP_EMAIL));
                            $('#txtMobile').val($.trim(data[0].EMP_MOBILE));
                            $('#selRole').val($.trim(data[0].ROLE));
                          
                        }
                    },
                    error: function (xhr, status, error) {
                        bootbox.alert("Error! " + xhr.status);
                    },
                });
            }
            $("#mdRegCus").modal({
                backdrop: 'static',
                keyboard: false
            });
        }
        else {
            bootbox.alert("Please select a row.");
        }
        return false;
    });
    $('#frmRegEmp').submit(function (e) {
        e.preventDefault();
        if ($('#frmRegEmp').smkValidate()) {
            var id = $.trim($('#txtEmpId').val());
            var action = $('#frmRegEmp').attr('data-action');
            var emp = {
                EMP_ID: id,
                EMP_NAME: $.trim($('#txtEmpName').val()),
                EMP_DEPT: $.trim($('#selDept').val()),
                EMP_EMAIL: $.trim($('#txtEmail').val()),
                EMP_MOBILE: $.trim($('#txtMobile').val()),
                ROLE: $.trim($('#selRole').val())
              
            };
           
            if (emp.EMP_ID) {
                $.ajax({
                    url: $('#hdUrl').val().replace("Action", "InsertUpdateEmployee"),
                    data: JSON.stringify({
                        EMP: emp,
                        ACTION: action
                    }),
                    type: 'POST',
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',
                    crossBrowser: true,
                    success: function (data, status) {
                        bootbox.alert(status);
                        tbEmp.ajax.reload();
                        $('#mdRegCus').modal('hide');
                        return false;
                    },
                    error: function (xhr, status, error) {
                        bootbox.alert("Error! " + xhr.status);
                    },
                });
            }
        }
        else {
            bootbox.alert('Invalid field');
        }

        return false;
    });
    $('#tbEmp').on('click', '.btnRemove', function () {
        $(this).closest('tr').remove();
        return false;
    });
    $('#btnAdd').click(function () {
        var sex = $('#selSex').val();
        var option = (sex == "Male") ? '<option value="Male" selected="selected">Male</option><option value="Female">Female</option>' : '<option value="Male">Male</option><option value="Female" selected="selected">Female</option>';
        var append = $(' <tr class="contact" data-id="">'

                                   + ' <td colspan="2">'
                                    + '    <input type="text" class="form-control person" value="' + $.trim($('#txtPerson').val()) + '" />'
                                    + '</td>'
                                    + '<td>'
                                     + '   <input type="text" class="form-control dob" value="' + $.trim($('#txtBirthday').val()) + '" />'
                                    + '</td>'
                                    + '<td>'
                                     + '   <input type="text" class="form-control pos" value="' + $.trim($('#txtPos').val()) + '"/>'
                                    + '</td>'
                                    + '<td>'
                                     + '  <input type="text" class="form-control mobile" value="' + $.trim($('#txtMobile').val()) + '"/>'

                                    + '</td>'
                                    + '<td>'
                                      + '   <input type="email" class="form-control email" value="' + $.trim($('#txtEmail').val()) + '"/>'
                                    + '</td>'
                                    + '<td colspan="4">'
                                       + ' <div class="input-group">'
                                            + '<select class="form-control sex">'
                                              + option
                                            + '</select>'
                                            + '<span class="input-group-btn">'
                                               + ' <button class="btn btn-danger btnRemove" type="button">'
                                                 + '   <span class="glyphicon glyphicon-minus" aria-hidden="true"></span>'
                                                + '</button>'
                                            + '</span>'
                                        + '</div>'
                                    + '</td>'

                                + '</tr>').insertAfter('#trAdd');

        $('.dob')
                       .datepicker({
                           format: 'yyyy-mm-dd'
                       }).on('changeDate', function (ev) {
                           $(this).datepicker('hide');
                       });
        return false;
    });
    function GetCusByEmpId(id) {
        $("#tbCus tbody").empty();
        $.ajax({
            url: $('#hdUrl').val().replace("Action", "GetCustomerById"),
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
                        $("#tbCus tbody").append('<tr><td><input type="checkbox" class="ck" data-id="'+data[i].ID+'" /></td><td class="cus">' + data[i].ID + '</td><td>' + data[i].NAME + '</td><td>' + data[i].CLASS + '</td></tr>');

                    }
                }

                return false;
            },
            error: function (xhr, status, error) {
                bootbox.alert("Error! " + xhr.status);
            },
        });
    }
});