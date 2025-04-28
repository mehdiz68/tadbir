$(document).ready(function () {

    //Change Users's Password----------------------------------
    $(".ChangePassword").click(function () {
        var newPass = $(this).parent().parent().find(".Password").val();
        if (newPass.length > 5) {
            $(".loading").show(); NProgress.start();
            var ajaxPostUrl = $(this).attr("data-url") + "&newPass=" + newPass;
            $.ajax({
                url: ajaxPostUrl,  //Server script to process data
                type: 'POST',
                //Ajax events
                success: function (data) {
                    swal({ title: "", text: data.status, type: "success", showCancelButton: false, confirmButtonClass: 'btn-success', confirmButtonText: 'بستن' });
                    $(".loading").hide(); NProgress.done();
                    $(".Password").val("");
                },
                error: function (data) {
                    swal({ title: "", text: data.status, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                    $(".loading").hide(); NProgress.done();
                },
                //Options to tell jQuery not to process data or worry about content-type.
                cache: false,
                contentType: false,
                processData: false,
                ascync: false
            });
        }
        else
            swal({ title: "", text: "حداقل 6 کارکتر وارد نمایید!", type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });


    });


    //Delete User----------------------------------
    $(".Delete").click(function () {
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
                          $(".loading").show(); NProgress.start();
                          var ajaxPostUrl = $this.attr("data-url"); 
                          $.ajax({
                              url: ajaxPostUrl,  //Server script to process data
                              type: 'POST',
                              //Ajax events
                              success: function (data) {
                                  swal({ title: "", text: data.status, type: "success", showCancelButton: false, confirmButtonClass: 'btn-success', confirmButtonText: 'بستن' });
                                  if (data.statusCode == 200) {
                                      $this.parent().parent().parent().remove();
                                      $(".Password").val("");
                                  }
                                  $(".loading").hide(); NProgress.done();
                              },
                              error: function (data) {
                                  swal({ title: "", text: data.status, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                                  $(".loading").hide(); NProgress.done();
                              },
                              //Options to tell jQuery not to process data or worry about content-type.
                              cache: false,
                              contentType: false,
                              processData: false,
                              ascync: false
                          });
                      } else {
                          swal.close();
                      }
                  });

       
    });

    //Disable User----------------------------------
    $(".Disable").click(function () {
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
                             $(".loading").show(); NProgress.start();
                             var ajaxPostUrl = $this.attr("data-url");
                             $.ajax({
                                 url: ajaxPostUrl,  //Server script to process data
                                 type: 'POST',
                                 //Ajax events
                                 success: function (data) {
                                     swal({ title: "", text: data.status, type: "success", showCancelButton: false, confirmButtonClass: 'btn-success', confirmButtonText: 'بستن' });
                                     if (data.statusCode == 200) {
                                         $this.hide();
                                         $this.parent().find(".Enable").show();
                                         $(".Password").val("");
                                     }
                                     $(".loading").hide(); NProgress.done();
                                 },
                                 error: function (data) {
                                     swal({ title: "", text: data.status, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                                     $(".loading").hide(); NProgress.done();
                                 },
                                 //Options to tell jQuery not to process data or worry about content-type.
                                 cache: false,
                                 contentType: false,
                                 processData: false,
                                 ascync: false
                             });
                         } else {
                             swal.close();
                         }
                     });


    });

    //Enable User----------------------------------
    $(".Enable").click(function () {
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
                            $(".loading").show(); NProgress.start();
                            var ajaxPostUrl = $this.attr("data-url");
                            $.ajax({
                                url: ajaxPostUrl,  //Server script to process data
                                type: 'POST',
                                //Ajax events
                                success: function (data) {
                                    swal({ title: "", text: data.status, type: "success", showCancelButton: false, confirmButtonClass: 'btn-success', confirmButtonText: 'بستن' });
                                    if (data.statusCode == 200) {
                                        $this.hide();
                                        $this.parent().find(".Disable").show();
                                        $(".Password").val("");
                                    }
                                    $(".loading").hide(); NProgress.done();
                                },
                                error: function (data) {
                                    swal({ title: "", text: data.status, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                                    $(".loading").hide(); NProgress.done();
                                },
                                //Options to tell jQuery not to process data or worry about content-type.
                                cache: false,
                                contentType: false,
                                processData: false,
                                ascync: false
                            });
                        } else {
                            swal.close();
                        }
                    });


    });

});