$(document).ready(function () {

    $("#ContentTypeId").change(function () {
        $(".loading").show();
        $.ajax({
            crossDomain: true,
            type: "POST",
            url: '/Admin/Contents/FilterCategory/',
            data: '{ContentTypeId:"' + $("#ContentTypeId option:selected").val() + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.success == 1) {
                    $("#TreeCategory").html(response.data);

                }

                $(".loading").hide();
            }
        });
    });
});
