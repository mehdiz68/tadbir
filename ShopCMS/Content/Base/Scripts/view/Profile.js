$(document).ready(function () {


    $("#Search-opener").click(function () {
        $("header .product_search_form").fadeToggle();    
    });
    if ($('#left-profile').length > 0) {
        var aTop = $("#left-profile").offset().top;
        $('html, body').animate({
            scrollTop: aTop - 80
        }, 800);
    }

    //Favorates-----------------------------------------------------------------------------------------
    $("#Favorates .row .favorate .options").click(function () {
        $(this).find(".delete").fadeToggle();
    });
    $("#Favorates .row .favorate .delete").click(function () {
        var $this = $(this);
        swal({
            title: "",
            text: "آیا اطمینان دارید؟",
            type: "warning",
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
                        url: "/Profile/RemoveFavorate/" + $this.attr("data-id"),
                        dataType: "json",
                        success: function (response) {
                            if (response.statusCode == 200) {
                                swal({ title: "", text: response.Message, type: "success", showCancelButton: false, confirmButtonClass: 'btn-success', confirmButtonText: 'بستن' });
                                $this.parent().parent().remove();

                            } else {
                                swal({ title: "", text: response.Message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                            }
                            $(".loading").hide();
                        },
                        error: function (e) {
                            swal({ title: "", text: e, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });

                            $(".loading").hide();

                        }
                    });
                } else {
                    swal.close();
                }
            });
    });

    //Notices-------------------------------------------------------------------------------------------
    $("#Notices .row .Notice .options").click(function () {
        $(this).find(".delete").fadeToggle();
    });
    $("#Notices .row .Notice .delete").click(function () {
        var $this = $(this);
        swal({
            title: "",
            text: "آیا اطمینان دارید؟",
            type: "warning",
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
                        url: "/Profile/RemoveNotice/" + $this.attr("data-id"),
                        dataType: "json",
                        success: function (response) {
                            if (response.statusCode == 200) {
                                swal({ title: "", text: response.Message, type: "success", showCancelButton: false, confirmButtonClass: 'btn-success', confirmButtonText: 'بستن' });
                                $this.parent().parent().remove();

                            } else {
                                swal({ title: "", text: response.Message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                            }
                            $(".loading").hide();
                        },
                        error: function (e) {
                            swal({ title: "", text: e, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });

                            $(".loading").hide();

                        }
                    });
                } else {
                    swal.close();
                }
            });
    });


    //MyComment-----------------------------------------------------------------------------------------
    $("#MyComment .row .Comment .options").click(function () {
        $(this).find(".delete").fadeToggle();
        $(this).find(".edit").fadeToggle();
    });
    $("#MyComment .row .Comment .delete").click(function () {
        var $this = $(this);
        swal({
            title: "",
            text: "آیا اطمینان دارید؟",
            type: "warning",
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
                        url: "/Profile/RemoveComment/" + $this.attr("data-id"),
                        dataType: "json",
                        success: function (response) {
                            if (response.statusCode == 200) {
                                swal({ title: "", text: response.Message, type: "success", showCancelButton: false, confirmButtonClass: 'btn-success', confirmButtonText: 'بستن' });
                                $this.parent().parent().parent().remove();

                            } else {
                                swal({ title: "", text: response.Message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                            }
                            $(".loading").hide();
                        },
                        error: function (e) {
                            swal({ title: "", text: e, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });

                            $(".loading").hide();

                        }
                    });
                } else {
                    swal.close();
                }
            });
    });

    //Adresses------------------------------------------------------------------------------------------
    $("#addresses .address .options").click(function () {
        $(this).find(".delete").fadeToggle();
    });
    $("#addresses  .address .delete").click(function () {
        var $this = $(this);
        swal({
            title: "",
            text: "آیا اطمینان دارید؟",
            type: "warning",
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
                        url: "/Profile/RemoveAddress/" + $this.attr("data-id"),
                        dataType: "json",
                        success: function (response) {
                            if (response.statusCode == 200) {
                                swal({ title: "", text: response.Message, type: "success", showCancelButton: false, confirmButtonClass: 'btn-success', confirmButtonText: 'بستن' });
                                $this.parent().parent().remove();

                            } else {
                                swal({ title: "", text: response.Message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                            }
                            $(".loading").hide();
                        },
                        error: function (e) {
                            swal({ title: "", text: e, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });

                            $(".loading").hide();

                        }
                    });
                } else {
                    swal.close();
                }
            });
    });

    //Message

    $("#Messages  .openMessage").click(function () {
        var $this = $(this);

        $.ajax({
            type: "POST",
            url: "/Profile/ReadMessage/" + $this.attr("data-id"),
            dataType: "json",
            success: function (response) {

            },
            error: function (e) {
                swal({ title: "", text: e, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });


            }
        });


    });


});