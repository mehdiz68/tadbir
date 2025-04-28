$(document).ready(function () {
    //---------------------------------
    $(".PageSizeSelector").change(function () {
        $("#PageSize").val($(this).val());
        $("#Search-Form").submit();
    });

    /* Delete One Record */
    $(".DeleteContent").click(function (e) {
        e.preventDefault();
        var $this = $(this);

        swal({
            title: "",
            text: "آیا مطمئن هستید؟",
            type: "success",
            showCancelButton: true,
            confirmButtonClass: "btn-success",
            confirmButtonText: "بلی",
            cancelButtonText: "خیر",
            cancelButtonClass: "btn-danger",
            closeOnConfirm: false,
            closeOnCancel: false
        },
            function (isConfirm) {
                if (isConfirm) {
                    $(".loading").show();
                    $.ajax({
                        type: "POST",
                        url: $this.attr("href"),
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (response) {
                            if (response.statusCode == 400) {
                                $this.parent().parent().remove();
                                swal({ title: "", text: response.message, type: "success", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                            }
                            else {
                                swal({ title: "", text: response.message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                            }

                            $(".loading").hide();
                        }
                    });
                    swal.close();
                } else {
                    swal.close();
                }
            });


    });
});