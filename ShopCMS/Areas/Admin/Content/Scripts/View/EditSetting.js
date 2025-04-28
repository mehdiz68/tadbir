$(document).ready(function () {
    var langid = 1;
    if ($("#LanguageId option:selected").val() != undefined)
        langid = $("#LanguageId option:selected").val();
    else
        langid = $("#LanguageId").val();

    $("#BtnEditContentType").click(function () {
        $(".loading").show();
        var checkedInputs = $(".ContentType input:checked");
        var checkedInputsArray = "";
        for (var i = 0; i < checkedInputs.length; i++)
            checkedInputsArray += checkedInputs.eq(i).attr("data-id")+",";
        if (checkedInputs.length > 0)
            checkedInputsArray = checkedInputsArray.substring(0, checkedInputsArray.lastIndexOf(","));

        $.ajax({
            ascync: false,
            type: "POST",
            url: "/Admin/Setting/EditContentTypes",
            data: '{Ids:"' + checkedInputsArray + '",langid:"' + langid + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 200) {
                    alert("ثبت شد");

                } else {
                    alert(response.Message);
                }
                $(".loading").hide();
            }
        });

    });

});