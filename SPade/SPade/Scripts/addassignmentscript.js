$(document).ready(function () {

    $(function () {
        $('input[name="DateRange"]').daterangepicker({

            autoUpdateInput: false,
            locale: {
                cancelLabel: 'Clear'
            }

        })
    });

    $("#DateRange").on('apply.daterangepicker', function (ev, picker) {

        var sd = picker.startDate.format('YYYY-MM-DD')
        var ed = picker.endDate.format('YYYY-MM-DD')

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

        var t = $("#cL").data("class-name");

        $("#SelectedClasses").val(t)

    })

    $("#classSelect").validate({

        rules: {

            classSelect: "required"
        },
        messages: {
            classSelect: "Please select a class"
        }
    })



})