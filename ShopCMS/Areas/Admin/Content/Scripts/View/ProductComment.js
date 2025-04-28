function sendData() {
    $(".loading").show();
    var myData = {};
    myData.id = $("input#Id").val();
    myData.title = $("input#Title").val();
    myData.text = tinymce.get('txt_Content').getContent();
    advantages = [];
    disAdvantages = [];
    $('.advantageData').each(function (i, obj) {
        advantages.push({
            id: $(obj).find("input[name='AdvantageId']").val(),
            title: $(obj).find("input[name='advantageText']").val(),
        });
    });

    $('.disAdvantageData').each(function (i, obj) {
        disAdvantages.push({
            id: $(obj).find("input[name='disAdvantageId']").val(),
            title: $(obj).find("input[name='disAdvantageText']").val(),
        });
    });

    var dataToSend = {
        'productComment': myData,
        'advantage': advantages,
        'disAdvantage': disAdvantages
    };

    jQuery.ajax({
        type: "POST",
        url: "/ProductComment/Edit",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(dataToSend),
        traditional: true,
        cache: false,
        success: function (data) {
            if (data.statusCode == 200) {
                window.location.href = '/Admin/ProductComment/Index';
            }
            else {
                $(".loading").hide();
                alert("دیتای مورد نظر یافت نشد");
            }
        },
        failure: function (errMsg) {
            $(".loading").hide();
            alert(errMsg.data);
        }
    });
}