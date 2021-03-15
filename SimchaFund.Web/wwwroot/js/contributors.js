$(() => {
    $(".rows").on('click', "#deposit", function () {        
        $("#hidden").remove();
        $("#deposit-header").text('Deposit for ' + $(this).data('name'))
        $("#deposit-modal").modal('show');
        let id = $(this).data('id');
        $("#deposit-form").append(`<input type="hidden" name="contributorId" value="${id}" id="hidden"/>`);
    })
    $("#save").on('click', function () {
        $("#deposit-modal").modal('hide');
    })

    $("#add").on('click', function () {
        $("#editId").remove();
        $("#cont-header").text('New Contributor');
        $("#initialDepositDiv").show();
        $("#cont-name").val('');
        $("#cont-cell").val('');
        $("#cont-always-include").prop('checked',false);
        $("#cont-date-created").val('');
        $("#new-cont").modal('show');
    })

    $("#save-cont").on('click', function () {
        $("#new-cont").modal('hide');
    })
    $("tbody").on('click', "#edit", function () {        
        $("#editId").remove();
        $("#initialDepositDiv").hide();
        $("#cont-header").text('Edit Contributor');
        $("#cont-name").val($(this).data('name'));
        $("#cont-cell").val($(this).data('cell'));
        let checkbox = $(this).data('always-include');        
        let date = $(this).data('date');
        $("#cont-always-include").prop('checked', checkbox=="True");
        $("#cont-date-created").val(date);        
        $("#contributor").append(`<input type="hidden" name="Id" value="${$(this).data('id')}" id="editId"/>`);
        $("#new-cont").modal('show');
    })
    $("#search").on('keyup', function () {
        let search = $(this).val();
        $("tbody tr").each(function () {
            let row = $(this);
            let cell = row.find("#name");
            if (!cell.text().toLowerCase().includes(search.toLowerCase())) {
                row.hide();
            }
            else {
                row.show();
            }
        })
    })
    
    $("#clear").on('click', function () {
        $("#search").val('');
        $(".rows").show();
    })
})

