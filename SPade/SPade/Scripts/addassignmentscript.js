$(document).ready(function () {

    var currentDate = moment().format('DD/MM/YYYY h:mm:ss A');

    //Start Date Selector JS
    $('input[name="StartDate"]').daterangepicker({
        opens: "center",
        singleDatePicker: true,
        timePicker: true,
        startDate: currentDate,
        minDate: currentDate,
        autoUpdateInput: true,
        locale: {
            cancelLabel: 'Clear',
            format: "DD/MM/YYYY h:mm:ss A"
        }
    })

    $("#StartDate").on('apply.daterangepicker', function (ev, picker) {

        //get the selected date
        var selectedDate = $("#StartDate").val();
        sDate = $("#StartDate").val();

        //reinit the due date WITH the selected date
        $('input[name="DueDate"]').daterangepicker({
            opens: "center",
            singleDatePicker: true,
            timePicker: true,
            startDate: selectedDate,
            minDate: selectedDate,
            autoUpdateInput: true,
            locale: {
                cancelLabel: 'Clear',
                format: 'DD/MM/YYYY h:mm:ss A'
            }
        })
    })

    $("#StartDate").on("cancel.daterangepicker", function (ev, picker) {

    })

    //Due Date Selector JS    
    $('input[name="DueDate"]').daterangepicker({
        opens: "center",
        singleDatePicker: true,
        timePicker: true,
        startDate: currentDate,
        minDate: currentDate,
        autoUpdateInput: true,
        locale: {
            cancelLabel: 'Clear',
            format: 'DD/MM/YYYY h:mm:ss A'
        }
    })

    $("#DueDate").on("cancel.daterangepicker", function (ev, picker) {

    })

    $("#SelectedClasses").click(function () {

        $("#classModal").modal("show");

    })

    $("#modalSelect").click(function () {

        $('input[type="checkbox"][name*="isSelected"]').each(function () {
            if (this.checked) {
                console.log($(this).data("class-name"));
            }
        })

        console.log("cyka");

        var t = $("#cL").data("class-name");

        $("#SelectedClasses").val(t)

    })

    $("#IsTestCasePresent").click(function () {

        if ($(this).is(":checked")) {

            $("#testCaseGroup").fadeIn();

        } else {

            $("#testCaseGroup").fadeOut();

        }
    })

    if ($("#IsTestCasePresent").is(":checked")) {

        $("#testCaseGroup").show();

    }


})