$(document).ready(function () {

    $(function () {

        var currentDate = moment().format('MM/DD/YYYY');

        $('input[name="DateRange"]').daterangepicker({
            opens: "center",
            startDate: currentDate,
            endDate: currentDate,
            minDate: currentDate,
            autoUpdateInput: false,
            locale: {
                cancelLabel: 'Clear'
            }
        })
    });

    $("#DateRange").on('apply.daterangepicker', function (ev, picker) {

        var sd = picker.startDate.format('DD-MM-YYYY');
        var ed = picker.endDate.format('DD-MM-YYYY');
        var currentDate = moment().format('DD-MM-YYYY');

        $("#DateRange").val(sd + " - " + ed);
        $('#StartDate').val(sd);
        $('#DueDate').val(ed);
    })

    $("#DateRange").on("cancel.daterangepicker", function (ev, picker) {

        $("#DateRange").val("");

    })

    $("#SelectedClasses").click(function () {

        $("#classModal").modal("show");

    })

    $("#modalSelect").click(function () {

        var l = [];

        $("input[id='cL']:checked"), function () {

            l.push($(this).val());

        }

        console.log(l);

    })

    $("#testCaseGroup").hide();

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