$(document).ready(function () {
    $("#SearchPageShow").click(function () {
        $("#SearchPage").slideToggle();
        $("#SearchPageShow").toggleClass("hilightSearch");
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



    /* Batch Operation */
    function GetSelectedRow() {
        var ids = "";
        $(".ContentRow").each(function () {
            var obj = $(this).find(".SelectAll");
            if (obj.is(":checked"))
                ids = ids + obj.attr("data-id") + ",";
        });
        if (ids != "")
            return ids.substring(0, ids.length - 1);
    }
    $(".RemoveContents").click(function () {
        if (confirm("آیا مطمئن هستید؟")) {
            $(".loading").show();
            $.ajax({
                type: "POST",
                url: "/Admin/Contents/RemoveContents",
                contentType: "application/json; charset=utf-8",
                data: '{ids:"' + GetSelectedRow() + '"}',
                dataType: "json",
                success: function (response) {
                    if (response.statusCode == 200) {
                        alert("انجام شد");
                        window.location.href = "/Admin/Contents/";
                    }
                    else {
                        alert(response.Message);
                    }

                    $(".loading").hide();
                }
            });
        }
    });
    $(".ActiveContents").click(function () {
        if (confirm("آیا مطمئن هستید؟")) {
            $(".loading").show();
            $.ajax({
                type: "POST",
                url: "/Admin/Contents/ActiveContents",
                contentType: "application/json; charset=utf-8",
                data: '{ids:"' + GetSelectedRow() + '"}',
                dataType: "json",
                success: function (response) {
                    if (response.statusCode == 200) {
                        alert("انجام شد");
                        window.location.href = "/Admin/Contents/";
                    }
                    else {
                        alert(response.Message);
                    }

                    $(".loading").hide();
                }
            });
        }
    });
    $(".DeActiveContents").click(function () {
        if (confirm("آیا مطمئن هستید؟")) {
            $(".loading").show();
            $.ajax({
                type: "POST",
                url: "/Admin/Contents/DeActiveContents",
                contentType: "application/json; charset=utf-8",
                data: '{ids:"' + GetSelectedRow() + '"}',
                dataType: "json",
                success: function (response) {
                    if (response.statusCode == 200) {
                        alert("انجام شد");
                        window.location.href = "/Admin/Contents/";
                    }
                    else {
                        alert(response.Message);
                    }

                    $(".loading").hide();
                }
            });
        }
    });

    //Select Category Of Selected Tree
    $("#tree li a").each(function () {
        if ($(this).next().hasClass("subItem")) {
            $(this).parent().prepend("<span class='PlusTree' style='position: relative;bottom: 0px;cursor:pointer'>   [+]   <span>");
            $(this).parent().find(".subItem").hide();
        }
    });
    $("#tree").on("click", ".PlusTree", function (e) {
        //show hide Child of list
        $(this).parent().prepend("<span class='MinusTree' style='position: relative;bottom: 0px;cursor:pointer'>   [-]   <span>");
        $(this).parent().find(".subItem").first().show();
        $(this).parent().find(".PlusTree").first().remove();

    });
    $("#tree").on("click", ".MinusTree", function (e) {
        //show hide Child of list
        $(this).parent().prepend("<span class='PlusTree' style='position: relative;bottom: 0px;cursor:pointer'>   [+]   <span>");
        $(this).parent().find(".subItem").first().hide();
        $(this).parent().find(".MinusTree").first().remove();
    });


    //Set Selected Tree
    var CurrentFolderId = $("#HdnCatId").val();
    if (CurrentFolderId != "") {
        if (CurrentFolderId > 0) {
            $("#tree li a").removeClass("treeActive");
            $("#tree li a[data-id='" + CurrentFolderId + "']").addClass("treeActive");

            $("#tree .subItem").hide();
            $("#tree li a[data-id='" + CurrentFolderId + "']").parents().each(function () {
                if ($(this).is(".subItem")) {
                    $(this).show();
                    $(this).prev().parent().prepend("<span class='MinusTree' style='position: relative;bottom: 6px;cursor:pointer'>   [-]   <span>");
                    $(this).prev().parent().find(".PlusTree").first().remove();
                }

            });
        }
        else {
            $(".tree ul > li a").removeClass("treeActive");
            $(".tree ul > li a[data-id='" + CurrentFolderId + "']").addClass("treeActive");
        }
    }

    var CurrentContentTypeId = $("#HdnContentTypeId").val();
    if (CurrentContentTypeId != "") {
        $(".tree ul > li a").removeClass("treeActive");
        $(".tree ul > li a[data-id='" + CurrentContentTypeId + "']").addClass("treeActive");
    }
});