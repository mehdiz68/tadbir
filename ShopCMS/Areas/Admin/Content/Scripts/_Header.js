$(document).ready(function () {
    $("form").on("submit", function () {
        if (!$(this).find(".export").hasClass("checked"))
            $(".loading").show();
    });
    $("form").bind("invalid-form.validate", function () {
        $(".loading").hide();
    });

    // NProgress
    if (typeof NProgress != 'undefined') {


        $("#UpdateSitemap").click(function (e) {
            e.preventDefault();
            var $this = $(this);
            $(".loading").show(); NProgress.start();
            $.ajax({
                type: "POST",
                url: $this.attr("href"),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.statusCode == 200) {

                        swal({
                            title: "",
                            text: response.message,
                            type: "success",
                            showCancelButton: true,
                            confirmButtonClass: "btn-success",
                            confirmButtonText: "فایل سایت مپ را می بینم",
                            cancelButtonText: "نیازی نیست !",
                            cancelButtonClass: "btn-danger",
                            closeOnConfirm: false,
                            closeOnCancel: false
                        },
                            function (isConfirm) {
                                if (isConfirm) {
                                    window.location = "/Sitemap.xml";
                                } else {
                                    swal.close();
                                }
                            });

                    }
                    else {
                        swal({ title: "", text: response.message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });

                    }
                    $(".loading").hide(); NProgress.done();
                }, error: function () {
                    swal({ title: "", text: "خطایی رخ داد !", type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                }
            });
        });

        $("#ClearCache").click(function (e) {
            e.preventDefault();
            var $this = $(this);
            $(".loading").show();
            $.ajax({
                type: "POST",
                url: $this.attr("href"),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.statusCode == 200) {

                        swal({ title: "", text: response.message, type: "success", showCancelButton: false, confirmButtonClass: 'btn-success', confirmButtonText: 'بستن' });
                    }
                    else {
                        swal({ title: "", text: response.message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                    }

                    $(".loading").hide();
                },
                error: function () {
                    $(".loading").hide();
                    swal({ title: "", text: "خطایی رخ داد !", type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });

                }
            });
        });
    }
});

//      document.querySelector('.sweet-1').onclick = function(){
//          swal("Here's a message!");
//      };

//document.querySelector('.sweet-2').onclick = function(){
//    swal("Here's a message!", "It's pretty, isn't it?")
//};

//document.querySelector('.sweet-3').onclick = function(){
//    swal("Good job!", "You clicked the button!", "success");
//};

//document.querySelector('.sweet-4').onclick = function(){
//    swal({
//        title: "Are you sure?",
//        text: "You will not be able to recover this imaginary file!",
//        type: "warning",
//        showCancelButton: true,
//        confirmButtonClass: 'btn-danger',
//        confirmButtonText: 'Yes, delete it!',
//        closeOnConfirm: false,
//        //closeOnCancel: false
//    },
//    function(){
//        swal("Deleted!", "Your imaginary file has been deleted!", "success");
//    });
//};

//document.querySelector('.sweet-5').onclick = function(){
//    swal({
//        title: "Are you sure?",
//        text: "You will not be able to recover this imaginary file!",
//        type: "warning",
//        showCancelButton: true,
//        confirmButtonClass: 'btn-danger',
//        confirmButtonText: 'Yes, delete it!',
//        cancelButtonText: "No, cancel plx!",
//        closeOnConfirm: false,
//        closeOnCancel: false
//    },
//    function(isConfirm){
//        if (isConfirm){
//            swal("Deleted!", "Your imaginary file has been deleted!", "success");
//        } else {
//            swal("Cancelled", "Your imaginary file is safe :)", "error");
//        }
//    });
//};

//document.querySelector('.sweet-6').onclick = function(){
//    swal({
//        title: "Sweet!",
//        text: "Here's a custom image.",
//        imageUrl: 'assets/thumbs-up.jpg'
//    });
//};

//document.querySelector('.sweet-7').onclick = function () {
//    swal({
//        title: "An input!",
//        text: "Write something interesting:",
//        type: "input",
//        showCancelButton: true,
//        closeOnConfirm: false,
//        animation: "slide-from-top",
//        inputPlaceholder: "Write something"
//    }, function (inputValue) {
//        if (inputValue === false) return false;
//        if (inputValue === "") {
//            swal.showInputError("You need to write something!");
//            return false
//        }
//        swal("Nice!", "You wrote: " + inputValue, "success");
//    });
//};

//document.querySelector('.sweet-8').onclick = function () {
//    swal({
//        title: "Ajax request example",
//        text: "Submit to run ajax request",
//        type: "info",
//        showCancelButton: true,
//        closeOnConfirm: false,
//        showLoaderOnConfirm: true
//    }, function () {
//        setTimeout(function () {
//            swal("Ajax request finished!");
//        }, 2000);
//    });
//};

//document.querySelector('.sweet-10').onclick = function(){
//    swal({
//        title: "Are you sure?",
//        text: "You will not be able to recover this imaginary file!",
//        type: "info",
//        showCancelButton: true,
//        confirmButtonClass: 'btn-primary',
//        confirmButtonText: 'Primary!'
//    });
//};

//document.querySelector('.sweet-11').onclick = function(){
//    swal({
//        title: "Are you sure?",
//        text: "You will not be able to recover this imaginary file!",
//        type: "info",
//        showCancelButton: true,
//        confirmButtonClass: 'btn-info',
//        confirmButtonText: 'Info!'
//    });
//};

//document.querySelector('.sweet-12').onclick = function(){
//    swal({
//        title: "Are you sure?",
//        text: "You will not be able to recover this imaginary file!",
//        type: "success",
//        showCancelButton: true,
//        confirmButtonClass: 'btn-success',
//        confirmButtonText: 'Success!'
//    });
//};

//document.querySelector('.sweet-13').onclick = function(){
//    swal({
//        title: "Are you sure?",
//        text: "You will not be able to recover this imaginary file!",
//        type: "warning",
//        showCancelButton: true,
//        confirmButtonClass: 'btn-warning',
//        confirmButtonText: 'Warning!'
//    });
//};

//document.querySelector('.sweet-14').onclick = function(){
//    swal({
//        title: "Are you sure?",
//        text: "You will not be able to recover this imaginary file!",
//        type: "error",
//        showCancelButton: true,
//        confirmButtonClass: 'btn-danger',
//        confirmButtonText: 'Danger!'
//    });
//};