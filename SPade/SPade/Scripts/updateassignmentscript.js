$(document).ready(function () {

    var currentDate = moment().format('DD/MM/YYYY h:mm A');

    $("#editor").wysiwyg();
    $("#editor").cleanHtml();

    var des = $("#Describe").val();
    $("#editor").html(des);

    var getStartDate = $("#StartDate").val();
    var getDueDate = $("#DueDate").val();

    setInitialClass();

    //Start Date Selector JS
    $('input[name="StartDate"]').daterangepicker({
        opens: "center",
        singleDatePicker: true,
        timePicker: true,
        startDate: getStartDate,
        minDate: getStartDate,
        autoUpdateInput: true,
        locale: {
            format: 'DD/MM/YYYY h:mm A'
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
                format: 'DD/MM/YYYY h:mm A'
            }
        })
    });

    //Due Date Selector JS    
    $('input[name="DueDate"]').daterangepicker({
        opens: "center",
        singleDatePicker: true,
        timePicker: true,
        startDate: getDueDate,
        minDate: getStartDate,
        autoUpdateInput: true,
        locale: {
            format: 'DD/MM/YYYY h:mm A'
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
                format: 'DD/MM/YYYY h:mm A'
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
                    format: 'DD/MM/YYYY h:mm A'
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
                format: 'DD/MM/YYYY h:mm A'
            }
        })
    })

    //used to reset the datepickers to the original dates from the assignment 
    $("#resetDefaultDate").click(function () {

        //remove the old pickers
        $("#StartDate").data("daterangepicker").remove()
        $("#DueDate").data("daterangepicker").remove()

        //bind the picker again
        $('input[name="StartDate"]').daterangepicker({
            opens: "center",
            singleDatePicker: true,
            timePicker: true,
            startDate: getStartDate,
            minDate: getStartDate,
            autoUpdateInput: true,
            locale: {
                format: 'DD/MM/YYYY h:mm A'
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
                    format: 'DD/MM/YYYY h:mm A'
                }
            })
        });

        //bind the picker again
        $('input[name="DueDate"]').daterangepicker({
            opens: "center",
            singleDatePicker: true,
            timePicker: true,
            startDate: getDueDate,
            minDate: getStartDate,
            autoUpdateInput: true,
            locale: {
                format: 'DD/MM/YYYY h:mm A'
            }
        })
    })

    //let users select if they need to update their existing solution
    if ($("#UpdateSolution").is(":checked")) {
        $("#solutionGroup").show();

    } else {
        $("#solutionGroup").hide();
    }

    //let users select if they need testcase with their solutions
    if ($("#IsTestCasePresent").is(":checked")) {
        $("#testCaseGroup").show();
    }

    $("#IsTestCasePresent").click(function () {
        if ($(this).is(":checked")) {
            $("#testCaseGroup").fadeIn();
        } else {
            $("#testCaseGroup").fadeOut();
        }
    })

    $("#UpdateSolution").click(function () {
        if ($(this).is(":checked")) {
            $("#solutionGroup").fadeIn();
        } else {
            $("#solutionGroup").fadeOut();
        }
    })

    //select class modal
    $("#SelectedClasses").click(function () {
        $("#classModal").modal("show");
    })

    function setInitialClass() {
        var selectedClasses = [];
        $('input[type="checkbox"][name*="isSelected"]').each(function () {
            if (this.checked) {
                var c = $(this).data("class-name");
                selectedClasses.push(c)
            }
        })
        $("#SelectedClasses").val(selectedClasses);
    }

    //to show which classes has been selected 
    $("#modalSelect").click(function () {
        var selectedClasses = [];
        $('input[type="checkbox"][name*="isSelected"]').each(function () {
            if (this.checked) {
                var c = $(this).data("class-name");
                selectedClasses.push(c)
            }
        })
        var t = $("#cL").data("class-name");
        $("#SelectedClasses").val(selectedClasses);
    })

    $("#submitBtn").click(function () {
        var qns = $("#editor").html();
        $("#Describe").val(qns)
    })
})