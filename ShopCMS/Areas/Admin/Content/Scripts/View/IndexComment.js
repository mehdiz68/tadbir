$(document).ready(function () {
    $("#SearchPageShow").click(function () {
        $("#SearchPage").slideToggle();
        $("#SearchPageShow").toggleClass("hilightSearch");
    });

    $(".Active").click(function (e) {
        var $this = $(this);
        e.preventDefault();

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
                    if (response.statusCode == 200) {
                                                                                    swal({ title: "", text: response.Message, type: "success", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });

                        window.location.href = $this.attr("data-url");
                    }
                    else {
                    swal({ title: "", text: response.Message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
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

    $(".DeActive").click(function (e) {
        var $this = $(this);
        e.preventDefault();

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
                    if (response.statusCode == 200) {
                                                                                    swal({ title: "", text: response.Message, type: "success", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });

                        window.location.href = $this.attr("data-url");
                    }
                    else {
                                                                                swal({ title: "", text: response.Message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });

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