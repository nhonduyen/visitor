$(document).ready(function () {
    $('#lhome').addClass('active');
    $('#from,#to,#txtTargetDate,#txtDate')
      .datepicker({
          format: 'yyyy-mm',
          viewMode: 'months'
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
                        from: $('#from').val()
                    });
                }
            }

        });
   
    $("#tbMainDefault").on("click", ".emp", function () {
        $('#frmRegVisit')[0].reset();
        $("h4").html("<span class='glyphicon glyphicon-edit'></span> MODIFY VISIT");
        var emp_id = $(this).attr('data-emid');
        var name = $(this).parent().next().text();
        $('#txtEmpId').val(emp_id);
        $('#txtEmpName').val(name);

        $.ajax({
            url: $('#hdUrl').val().replace("Action", "GetTarget"),
            data: JSON.stringify({
                ID: emp_id,
                DATE: $('#from').val()
            }),
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            crossBrowser: true,
            success: function (data, status) {
                if (data) {
                    $('#txtTarget').val(data);
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

        return false;
    });
   
    $('#selCustomer').change(function () {
        var id = $(this).val();

        if ($(this).val().length >= 5) {
            $('#selContact option').remove();
            $.ajax({
                url: $('#hdUrl').val().replace("Action", "GetContact"),
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
                            value: $.trim(data.CONTACTS[i].ID),
                            text: $.trim(data.CONTACTS[i].NAME)
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
   
    $('#frmRegVisit').submit(function (e) {
        e.preventDefault();
        var role = $('#username').attr('data-role');
        //if ($('#frmRegOrder').smkValidate()) {
        var taget = {
            EMP_ID: $('#txtEmpId').val(),
            VISIT_PLAN_MONTH: $('#txtTargetDate').val(),
            VISIT_TARGET: $('#txtTarget').val()
        };

      if (parseInt(role) > 0){
        $.ajax({
            url: $('#hdUrl').val().replace("Action", "InsertUpdateTarget"),
            data: JSON.stringify({
                TARGET: taget,
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
                }
                if (data == -1) {
                    alert('Please select contactor');
                }
                if (data == 0) {
                    alert('Only manager can set target');
                }
                return false;
            },
            error: function (xhr, status, error) {
                bootbox.alert("Error! " + xhr.status);
            },
        });

        }
        else {
            bootbox.alert('You cannot set target');
        }

        return false;
    });

    $('#btnSearch').click(function () {
        tbClaim.draw();
        return false;
    });
    $('#btnExport').click(function () {
        var month = $('#from').val();
        if (month) {
            var url = $(this).attr('data-url') + "?month=" + month;
            window.location.replace(url);
        }
        return false;
    });
});