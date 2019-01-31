$(document).ready(function () {
    $('#lcustomer').addClass('active');
    $('#month,#txtFinish,#txtDate,#txtVisitDate')
     .datepicker({
         format: 'yyyy-mm',
         viewMode: 'months'
     }).on('changeDate', function (ev) {
         $(this).datepicker('hide');
     });
    $('#txtOrderDate,#txtBirthday')
    .datepicker({
        format: 'yyyy-mm-dd'
    }).on('changeDate', function (ev) {
        $(this).datepicker('hide');
    });
    $('#btnRegCus').click(function () {
        $("h4").html("<span class='glyphicon glyphicon-edit'></span> NEW CUSTOMER");
        $('#btnChange').attr('data-id', '');
        $('#frmRegCus').attr('data-id', '');
        $('#frmRegCus')[0].reset();
        $('.contact').remove();
        $("#mdRegCus").modal({
            backdrop: 'static',
            keyboard: false
        });
        $.ajax({
            url: $('#hdUrl').val().replace("Action", "GenerateCode"),
            data: JSON.stringify({

            }),
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            crossBrowser: true,
            success: function (data, status) {
                $('#txtCusId').val(data);
                return false;
            },
            error: function (xhr, status, error) {
                bootbox.alert("Error! " + xhr.status);
            },
        });
        return false;
    });
    var tbCustomer = $('#tbMainDefault').DataTable(
           {
               sort: false,
               "processing": true,
               "serverSide": true,
               "searching": true,
               ajax: {
                   type: "POST",
                   contentType: "application/json",
                   url: $('#hdUrl').val().replace("Action", "GetCustomer"),
                   data: function (d) {
                       // note: d is created by datatable, the structure of d is the same with DataTableParameters model above

                       return JSON.stringify({ dataTableParameters: d });
                   }
               }

           });
    $('#btnDel').click(function () {
        if ($("input:checkbox:checked").length > 0) {
            var id = $("#tbMainDefault tr").find("input[type='checkbox']:checked").attr('id');

            if (id) {
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
                            tbCustomer.ajax.reload();
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
    $('#btnChange').click(function () {
        var id = $.trim($('#txtCusId').val());
        var oldId = $('#btnChange').attr('data-id');
        if (id == oldId) {
            alert("2 Id is the same");
            return false;
        }
        if (oldId && id != oldId) {
            $.ajax({
                url: $('#hdUrl').val().replace("Action", "ChangeId"),
                data: JSON.stringify({
                    NewId: id,
                    OldId: oldId
                }),
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                crossBrowser: true,
                success: function (data, status) {
                    if (data > 0) {
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

        return false;
    });
    $("#btnModify").click(function () {
        if ($("input:checkbox:checked").length > 0) {
            $("h4").html("<span class='glyphicon glyphicon-edit'></span> MODIFY CUSTOMER");
            var id = $("#tbMainDefault tr").find("input[type='checkbox']:checked").attr('id');
            $('#frmRegCus').attr('data-id', id);
            $('#btnChange').attr('data-id', id);
            $('#frmRegCus')[0].reset();
            $('.contact').remove();
            if (id) {
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
                                $('#txtCusId').val($.trim(data[i].ID));
                                $('#txtCusName').val($.trim(data[i].NAME));
                                $('#selClass').val($.trim(data[i].CLASS));
                                $('#txtAddress').val($.trim(data[i].ADDRESS));
                                $('#txtTel').val($.trim(data[i].TEL));
                                $('#txtLoc').val($.trim(data[i].LOCATION));
                                $('#txtFax').val($.trim(data[i].FAX));
                                $('#txtEs').val($.trim(data[i].ESTABLE));
                                $('#txtContact').val($.trim(data[i].CONTACT));
                                $('#txtApp').val($.trim(data[i].APPLICATION));
                                $('#txtDesc').val($.trim(data[i].SPEC));
                                $('#selGroup').val($.trim(data[i].TYPE));
                            }
                        }

                        return false;
                    },
                    error: function (xhr, status, error) {
                        bootbox.alert("Error! " + xhr.status);
                    },
                });
                $.ajax({
                    url: $('#hdUrl').val().replace("Action", "GetContactByCusId"),
                    data: JSON.stringify({
                        ID: id
                    }),
                    type: 'POST',
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',
                    crossBrowser: true,
                    success: function (data, status) {
                        for (var i = 0; i < data.length; i++) {
                            var sex = data[i].SEX;
                            var option = (sex == "Male") ? '<option value="Male" selected="selected">Male</option><option value="Female">Female</option>' : '<option value="Male">Male</option><option value="Female" selected="selected">Female</option>';
                            var append = $(' <tr class="contact" data-id="' + data[i].ID + '">'

                                                       + ' <td colspan="2">'
                                                        + '    <input type="text" class="form-control person" value="' + $.trim(data[i].NAME) + '" />'
                                                        + '</td>'
                                                        + '<td>'
                                                         + '   <input type="text" class="form-control dob" value="' + $.trim(data[i].DOB) + '" />'
                                                        + '</td>'
                                                        + '<td>'
                                                         + '   <input type="text" class="form-control pos" value="' + $.trim(data[i].POSITION) + '"/>'
                                                        + '</td>'
                                                        + '<td>'
                                                         + '  <input type="text" class="form-control mobile" value="' + $.trim(data[i].MOBILE) + '"/>'
                                                        + '</td>'
                                                        + '<td>'
                                                        + '   <input type="email" class="form-control email" value="' + $.trim(data[i].EMAIL) + '"/>'

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
                        }
                        return false;
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
    $('#frmRegCus').submit(function (e) {
        e.preventDefault();
        var id = $.trim($('#txtCusId').val());
        var action = $('#frmRegCus').attr('data-id').length > 0 ? 1 : 0;
        var customer = {
            ID: $.trim($('#txtCusId').val()),
            NAME: $.trim($('#txtCusName').val()),
            CLASS: $.trim($('#selClass').val()),
            ADDRESS: $.trim($('#txtAddress').val()),
            TEL: $.trim($('#txtTel').val()),
            LOCATION: $.trim($('#txtLoc').val()),
            FAX: $.trim($('#txtFax').val()),
            ESTABLE: $.trim($('#txtEs').val()),
            CONTACT: $.trim($('#txtContact').val()),
            APPLICATION: $.trim($('#txtApp').val()),
            SPEC: $.trim($('#txtDesc').val()),
            TYPE: $.trim($('#selGroup').val())
        };
        var contacts = [];
        if ($('#txtPerson').val()) {
            var contact1 = {
                ID: '',
                CUSTOMER_ID: id,
                NAME: $('#txtPerson').val(),
                DOB: $('#txtBirthday').val(),
                POSITION: $('#txtPos').val(),
                EMAIL: $('#txtEmail').val(),
                MOBILE: $('#txtMobile').val(),
                SEX: $('#selSex').val()
            };
            contacts.push(contact1);
        }
        $.each($(".contact"), function () {
            var contact = {
                ID: $(this).attr('data-id'),
                CUSTOMER_ID: id,
                NAME: $(this).find('.person').val(),
                DOB: $(this).find('.dob').val(),
                POSITION: $(this).find('.pos').val(),
                EMAIL: $(this).find('.email').val(),
                MOBILE: $(this).find('.mobile').val(),
                SEX: $(this).find('.sex').val()
            };
            contacts.push(contact);
        });
        if (customer.ID) {
            $.ajax({
                url: $('#hdUrl').val().replace("Action", "InsertUpdateCustomer"),
                data: JSON.stringify({
                    CUS: customer,
                    CONTACTS: contacts,
                    ACTION: action
                }),
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                crossBrowser: true,
                success: function (data, status) {
                    bootbox.alert(status);
                    tbCustomer.ajax.reload();
                    $('#mdRegCus').modal('hide');
                    $('.contact').remove();
                    return false;
                },
                error: function (xhr, status, error) {
                    bootbox.alert("Error! " + xhr.status);
                },
            });
        }


        return false;
    });
    $('#tbCustomer').on('click', '.btnRemove', function () {
        var id = $(this).closest('.contact').attr('data-id');
        if (id) {
            var cfm = confirm('Are you sure you want to delete this contact');
            if (cfm) {
                $.ajax({
                    url: $('#hdUrl').val().replace("Action", "DeleteContact"),
                    data: JSON.stringify({
                        ID: id
                    }),
                    type: 'POST',
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',
                    crossBrowser: true,
                    success: function (data, status) {
                        alert(status);
                        return false;
                    },
                    error: function (xhr, status, error) {
                        bootbox.alert("Error! " + xhr.status);
                    },
                });
            }
        }
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

        $('#txtPerson,#txtBirthday,#txtPos,#txtEmail,#txtMobile').val('');
        $('.dob')
                       .datepicker({
                           format: 'yyyy-mm-dd'
                       }).on('changeDate', function (ev) {
                           $(this).datepicker('hide');
                       });
        return false;
    });
    $('#btnAddEndUser').click(function () {
        $("#mdEnd").modal({
            backdrop: 'static',
            keyboard: false
        });
        return false;
    });
    $('#tbMainDefault').on('click', '.end', function () {
        $('#tbEndUser tbody').empty();
        var id = $(this).attr('id');
        var count = parseInt($(this).children('span').text());
        if (count > 0) {
            $.ajax({
                url: $('#hdUrl').val().replace("Action", "GetListEndUser"),
                data: JSON.stringify({
                    CUS_ID: id
                }),
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                crossBrowser: true,
                success: function (data, status) {
                    if (data.length > 0) {
                        for (var i = 0; i < data.length; i++) {
                            var row = '<tr><td>' + data[i].CUS_ID + '</td><td>' + data[i].END_USER_ID + '</td><td>' + data[i].NAME + '</td></tr>';
                            $('#tbEndUser tbody').append(row);
                        }
                        $("#mdListEnd").modal({
                            backdrop: 'static',
                            keyboard: false
                        });
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
    $('#frmEnd').submit(function () {
        var end = {
            CUS_ID: $.trim($('#txtCus_Id').val()),
            END_USER_ID: $.trim($('#txtEndId').val()),
            NAME: $.trim($('#txtEndName').val())
        };
        $.ajax({
            url: $('#hdUrl').val().replace("Action", "AddEndUser"),
            data: JSON.stringify({
                END: end
            }),
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            crossBrowser: true,
            success: function (data, status) {
                if (data == -1) {
                    alert("End User Exists");
                    return false;
                }
                alert(status);
                return false;
            },
            error: function (xhr, status, error) {
                bootbox.alert("Error! " + xhr.status);
            },
        });
        return false;
    });
});