
$(document).ready(function () {
    $("#ProvinceId").change(function () {
        $(".loading").show();
        $.ajax({
            type: "Get",
            url: "/Profile/GetCities?ProvinceId=" + $("#ProvinceId").val(),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                $("#CityId option").remove();
                for (var i = 0; i < response.data.length; i++) {

                    $("#CityId").append("<option value='" + response.data[i].Id + "'>" + response.data[i].Name + "</option>");
                }

                $(".loading").hide();
            }
        });
    });
    $("#FirstName").on('change keyup paste keydown', function (e) {
        if (just_persian(e.key, e.keyCode) === false) {
            e.preventDefault();
            alert("کیبرد خود را روی فارسی قرار دهید");
        }
    });
    $("#LastName").on('change keyup paste keydown', function (e) {
        if (just_persian(e.key, e.keyCode) === false) {
            e.preventDefault();
            alert("کیبرد خود را روی فارسی قرار دهید");
        }
    });
    //$("#Address").on('change keyup paste keydown', function (e) {
    //    if (just_persian(e.key, e.keyCode) === false) {
    //        e.preventDefault();
    //        alert("کیبرد خود را روی فارسی قرار دهید");
    //    }
    //});

});