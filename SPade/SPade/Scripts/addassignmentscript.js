$(document).ready(function () {

    $(function () {
        $('input[name="daterange"]').daterangepicker({

            autoUpdateInput: false,
            locale: {
                cancelLabel: 'Clear'
            }

        })
    });

    $("#daterange").on('apply.daterangepicker', function (ev, picker) {

        var sd = picker.startDate.format('YYYY-MM-DD')
        var ed = picker.endDate.format('YYYY-MM-DD')

        $("#daterange").val(sd + " - " + ed);
        $('#StartDate').val(sd);
        $('#DueDate').val(ed);
    })

    $("#daterange").on("cancel.daterangepicker", function (ev, picker) {

        $("#daterange").val("");

    })

    $("#classSelect").click(function () {

        $("#classModal").modal("show");

    })

    $("#modalSelect").click(function () {

        var t = $("#cL").data("class-name");

        $("#classSelect").val(t)

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