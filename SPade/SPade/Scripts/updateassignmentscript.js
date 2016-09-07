$(document).ready(function () {

    var currentDate = moment().format('DD/MM/YYYY h:mm A');

    $("#editor").wysiwyg();
    $("#editor").cleanHtml();

    var des = $("#Describe").val();
    $("#editor").html(des);

    var getStartDate = $("#StartDate").val();
    var getDueDate = $("#DueDate").val();

    setInitialClass();

    $("#deleteProgress").hide();

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
    });

    //bind action when apply is clicked
    $('input[name="StartDate"]').on('apply.daterangepicker', function (ev, picker) {

        //if user selects a start date, reinit duedate selector with new values 
        var selectedDate = $('#StartDate').val();

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
        });
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
    });

    //used to reset the datepickers 
    $("#resetBtn").click(function () {

        //remove the old pickers
        $("#StartDate").data("daterangepicker").remove();
        $("#DueDate").data("daterangepicker").remove();

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
        });
        //set the action again
        $('input[name="StartDate"]').on('apply.daterangepicker', function (ev, picker) {

            //if user selects a start date, reinit duedate selector with new values 
            var selectedDate = $('#StartDate').val();

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
            });
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
        });
    });

    //used to reset the datepickers to the original dates from the assignment 
    $("#resetDefaultDate").click(function () {

        //remove the old pickers
        $("#StartDate").data("daterangepicker").remove();
        $("#DueDate").data("daterangepicker").remove();

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
        });
        //set the action again
        $('input[name="StartDate"]').on('apply.daterangepicker', function (ev, picker) {

            //if user selects a start date, reinit duedate selector with new values 
            var selectedDate = $('#StartDate').val();

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
            });
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
        });
    });

    //to make the file upload shown or hidden when post back to the page
    if ($("#UpdateSolution").is(":checked")) {
        $("#solutionGroup").show();

    } else {
        $("#solutionGroup").hide();
    }

    //to make the test case upload shown or hidden during post back 
    if ($("#IsTestCasePresent").is(":checked")) {
        $("#testCaseGroup").show();
    } else {
        $("#testCaseGroup").hide();
    }

    //let users select if they need to update testcase
    $("#IsTestCasePresent").click(function () {
        if ($(this).is(":checked")) {
            $("#testCaseGroup").fadeIn();
        } else {
            $("#testCaseGroup").fadeOut();
        }
    });

    //let users select if they need to update the solution
    $("#UpdateSolution").click(function () {
        if ($(this).is(":checked")) {
            $("#solutionGroup").fadeIn();
        } else {
            $("#solutionGroup").fadeOut();
        }
    });

    //select class modal
    $("#SelectedClasses").click(function () {

        //setting modal options
        $("#classModal").modal({
            backdrop: 'static',
            keyboard: false
        });

        $("#classModal").modal("show");
    });

    function setInitialClass() {
        var selectedClasses = [];
        $('input[type="checkbox"][name*="isSelected"]').each(function () {
            if (this.checked) {
                var c = $(this).data("class-name");
                selectedClasses.push(c);
            }
        });
        $("#SelectedClasses").val(selectedClasses);
    }

    //to show which classes has been selected 
    $("#modalSelect").click(function () {
        var selectedClasses = [];
        $('input[type="checkbox"][name*="isSelected"]').each(function () {
            if (this.checked) {
                var c = $(this).data("class-name");
                selectedClasses.push(c);
            }
        });
        $("#SelectedClasses").val(selectedClasses);
    });

    $("#updateBtn").click(function () {
        var qns = $("#editor").html();
        $("#Describe").val(qns);

        //setting modal options
        $("#progressModal").modal({
            backdrop: 'static',
            keyboard: false
        });
        $("#progressModal").modal("show");
    });

    //for the deletion modal
    $("#deleteBtn").click(function () {
        $("#deleteMsg").hide();
        $("#deleteBtnGrp").hide();
        $("#deleteProgress").show();
    });

});