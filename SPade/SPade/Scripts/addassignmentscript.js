$(document).ready(function () {

    var currentDate = moment().format('DD/MM/YYYY h:mm A');

    $("#editor").wysiwyg();
    $("#editor").cleanHtml();

    var IsPostBack = $("#IsPostBack").val();

    InitStartDatePicker();
    InitDueDatePicker();
    AppendTextToEditor();

    //Start Date Selector JS
    function InitStartDatePicker() {

        //Inits with default value if NOT postback
        if (IsPostBack == 0) {

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
        }

        //Inits with previous value IF postback
        if (IsPostBack == 1) {

            var getStartDate = $("#StartDate").val();
            var getDueDate = $("#DueDate").val();

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
        }

    }

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
    function InitDueDatePicker() {

        //Init due date picker with default value if NOT postback
        if (IsPostBack == 0) {

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
        }

        //Init start date picker with previous value IF postback
        if (IsPostBack == 1) {

            var getStartDate = $("#StartDate").val();
            var getDueDate = $("#DueDate").val();

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
        }

    }

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

    //show the modal
    $("#SelectedClasses").click(function () {
        $("#classModal").modal({
            backdrop: 'static',
            keyboard: false
        });
        $("#classModal").modal("show");
    });

    //to show which class has been selected
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

    //let users select if they need testcase or not 
    $("#IsTestCasePresent").click(function () {
        if ($(this).is(":checked")) {
            $("#testCaseGroup").fadeIn();
        } else {
            $("#testCaseGroup").fadeOut();
        }
    });

    //show or hide the test case upload during postback
    if ($("#IsTestCasePresent").is(":checked")) {
        $("#testCaseGroup").show();
    } else {
        $("#testCaseGroup").hide();
    }
	

    //we reappend the text to the editor after postback
    function AppendTextToEditor() {

        if (IsPostBack == 1) {
            var des = $("#Describe").val();
            $("#editor").html(des);
        }
    }

    $(function () {
        $("#submitAssignmentForm").submit(function () {
            //only show the progress modal when form is validated
            if ($(this).valid()) {
                var qns = $("#editor").html();
                $("#Describe").val(qns);
                $("#progressModal").modal("show");
            }
        });
    });
});