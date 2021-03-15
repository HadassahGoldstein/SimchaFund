$(() => {
    $("#add-simcha").on('click', function () {
        $(".modal").modal('show');
    })
    $("#cancel").on('click', function () {
        $(".modal").modal('hide');
    })
    $("#save").on('click', function () {
        $(".modal").modal('hide');
    })
})