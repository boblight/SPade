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

    //$("#testCase").validate({

    //    rules: {
    //        testCase: {
    //            required: true,
    //            extension: "zip"
    //        }
    //    }

    //});
    //$("#testCase").removeAttr('novalidate');

    $("body").on("click", "#submitBtn", function () {

        var allowedFiles = [".xml"];
        var uploadedTestCaseFile = $("#fileList");
        var spanMsg = $("#testCaseError");
        var regex = new RegExp("([a-zA-Z0-9\s_\\.\-:])+(" + allowedFiles.join('|') + ")$");

        if (!regex.test(uploadedTestCaseFile.val().toLowerCase())) {
            spanMsg.html("Please upload your test case in .xml format !");
            return false
        }
        spanMsg.html("");
        return true;
    })


})