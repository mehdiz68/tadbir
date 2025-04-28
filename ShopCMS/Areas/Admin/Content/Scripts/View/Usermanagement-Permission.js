$(document).ready(function () {

    //Get checked cheboxes and send to action----------------------------------
    $("#AddPermission").click(function () {
        var $this = $(this);
        var AllowInsertId = "";
        var AllowUpdateId = "";
        var AllowDeleteId = "";
        var NotificationEmailId = "";
        $("#Permissions .PermissionsItem").each(function () {
            if ($(this).find(".AllowInsert").is(':checked')) {
                AllowInsertId +=$(this).find(".AllowInsert").attr("data-id")+"-"+$(this).find(".NotificationEmail").is(':checked')+ ",";
            }
            if ($(this).find(".AllowUpdate").is(':checked')) {
                AllowUpdateId += $(this).find(".AllowUpdate").attr("data-id") + "-" + $(this).find(".NotificationEmail").is(':checked') + ",";
            }
            if ($(this).find(".AllowDelete").is(':checked')) {
                AllowDeleteId += $(this).find(".AllowDelete").attr("data-id") + "-" + $(this).find(".NotificationEmail").is(':checked') + ",";
            }

        });

        if (AllowInsertId.length > 0)
            AllowInsertId = AllowInsertId.substring(0, AllowInsertId.lastIndexOf(","));
        if (AllowUpdateId.length > 0)
            AllowUpdateId = AllowUpdateId.substring(0, AllowUpdateId.lastIndexOf(","));
        if (AllowDeleteId.length > 0)
            AllowDeleteId = AllowDeleteId.substring(0, AllowDeleteId.lastIndexOf(","));

        $(".loading").show(); NProgress.start();
        var ajaxPostUrl = $(this).attr("data-url");
        var BackUrl = $(this).attr("data-urlBack");
        $.ajax({
            ascync: false,
            url: ajaxPostUrl,  //Server script to process data
            type: 'POST',
            data: JSON.stringify({ 'AllowInsertIds': AllowInsertId, 'AllowUpdateIds': AllowUpdateId, 'AllowDeleteIds': AllowDeleteId,'UserId': $("#UserId").val() }),
            //Ajax events
            success: function (data) {
                swal({ title: "", text: data.status, type: "success", showCancelButton: false, confirmButtonClass: 'btn-success', confirmButtonText: 'بستن' });

                if (data.statusCode == 200) {
                    window.location.href = BackUrl;
                }
                $(".loading").hide(); NProgress.start();
            },
            error: function (data) {
                swal({ title: "", text: data.status, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });

                $(".loading").hide();NProgress.start();
            },
            //Options to tell jQuery not to process data or worry about content-type.
            cache: false,
            contentType: "application/json",
            dataType: "json",
        });
    });


});