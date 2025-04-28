$(document).ready(function () {

    $("#ContentTypeId").change(function () {
        $(".loading").show();
        $.ajax({
            crossDomain: true,
            type: "POST",
            url: 'Admin/Categories/FilterCategory/',
            data: '{ContentTypeId:"' + $("#ContentTypeId option:selected").val() + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.success == 1) {
                    $("#ParrentId option").remove();
                    $("#ParrentId").append("<option value>--ندارد--</option>");
                    for (var i = 0; i < response.data.length; i++) {
                        $("#ParrentId").append("<option value='" + response.data[i].Value + "'>" + response.data[i].Text + "</option>");
                    }

                }

                $(".loading").hide();
            }
        });
    });
});
