$(document).ready(function () {

    var currentDate = moment().format('DD/MM/YYYY h:mm:ss A');

    $("#StartDate").val(currentDate);


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