$(function () {
    $('input[name="daterange"]').daterangepicker();
});

$("#daterange").on('apply.daterangepicker', function (ev, picker) {

    console.log(picker.startDate.format('YYYY-MM-DD'));
    console.log(picker.endDate.format('YYYY-MM-DD'));
    var currentTime = moment().format('HH:mm:ss');
    console.log(currentTime);
    var sd = picker.startDate.format('YYYY-MM-DD');
    var ed = picker.endDate.format('YYYY-MM-DD');

    $('#StartDate').val(sd)
    $('#DueDate').val(ed)

})

$("#classSelect").click(function () {

    $("#classModal").modal("show");

})

$(document).on("click", "#classSelect", function (e) {

    $("classModal").modal("show");


})