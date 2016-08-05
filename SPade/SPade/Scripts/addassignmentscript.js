$(document).ready(function () {

    var currentDate = moment().format('DD/MM/YYYY h:mm:ss A');

    $("#editor").wysiwyg();
    $("#editor").cleanHtml();

    //Start Date Selector JS
    $('input[name="StartDate"]').daterangepicker({
        opens: "center",
        singleDatePicker: true,
        timePicker: true,
        startDate: currentDate,
        minDate: currentDate,
        autoUpdateInput: true,
        locale: {
            format: 'DD/MM/YYYY h:mm:ss A'
        }
    })

    //bind action when apply is clicked
    $('input[name="StartDate"]').on('apply.daterangepicker', function (ev, picker) {

        //if user selects a start date, reinit duedate selector with new values 
        var selectedDate = $('#StartDate').val()

        $('input[name="DueDate"]').daterangepicker({
            opens: "center",
            singleDatePicker: true,
            timePicker: true,
            startDate: selectedDate,
            minDate: selectedDate,
            autoUpdateInput: true,
            locale: {
                format: 'DD/MM/YYYY h:mm:ss A'
            }
        })
    });

    //Due Date Selector JS    
    $('input[name="DueDate"]').daterangepicker({
        opens: "center",
        singleDatePicker: true,
        timePicker: true,
        startDate: currentDate,
        minDate: currentDate,
        autoUpdateInput: true,
        locale: {
            format: 'DD/MM/YYYY h:mm:ss A'
        }
    })

    //used to reset the datepickers 
    $("#resetBtn").click(function () {

        //remove the old pickers
        $("#StartDate").data("daterangepicker").remove()
        $("#DueDate").data("daterangepicker").remove()

        //bind the picker again
        $('input[name="StartDate"]').daterangepicker({
            opens: "center",
            singleDatePicker: true,
            timePicker: true,
            startDate: currentDate,
            minDate: currentDate,
            autoUpdateInput: true,
            locale: {
                format: 'DD/MM/YYYY h:mm:ss A'
            }
        })
        //set the action again
        $('input[name="StartDate"]').on('apply.daterangepicker', function (ev, picker) {

            //if user selects a start date, reinit duedate selector with new values 
            var selectedDate = $('#StartDate').val()

            $('input[name="DueDate"]').daterangepicker({
                opens: "center",
                singleDatePicker: true,
                timePicker: true,
                startDate: selectedDate,
                minDate: selectedDate,
                autoUpdateInput: true,
                locale: {
                    format: 'DD/MM/YYYY h:mm:ss A'
                }
            })
        });

        //bind the picker again
        $('input[name="DueDate"]').daterangepicker({
            opens: "center",
            singleDatePicker: true,
            timePicker: true,
            startDate: currentDate,
            minDate: currentDate,
            autoUpdateInput: true,
            locale: {
                format: 'DD/MM/YYYY h:mm:ss A'
            }
        })
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

    $("#submitBtn").click(function () {
        var qns = $("#editor").html();
        $("#Describe").val(qns)
    })
})