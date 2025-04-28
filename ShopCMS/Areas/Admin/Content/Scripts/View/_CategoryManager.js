$(document).ready(function () {

    $("body").on("change", "#ProductTypeId", function () {
        var typeid = $("#ProductTypeId").val();
        $.ajax({
            type: "GET",
            url: '/Admin/Products/GetCategory/?ProductTypeId=' + typeid,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 200) {
                    $("#ProductCatId").val(null);

                    var r = $(".CategoryManager select").filter(function () {
                        return parseInt($(this).attr("data-level")) > 1;
                    });
                    r.remove();
                    $(".CategoryManager select option").remove();
                    var html = "<option id='0' value='Null'>هیچکدام</option>";
                    $(".CategoryManager select").append(html);
                    for (var i = 0; i < response.data.length; i++) {
                        html = "<option id='Cat" + response.data[i].Id + "' value='" + response.data[i].Id + "'>" + response.data[i].Title + "</option>";
                        $(".CategoryManager select").append(html);
                    }
                }
            }

        });
    });
    $("body").on("change", "#OfferProductTypeId", function () {
        var typeid = $("#OfferProductTypeId").val();
        $.ajax({
            type: "GET",
            url: '/Admin/Products/GetOfferCategory/?ProductTypeId=' + typeid + "&OfferId=" + $("#OfferId").val(),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 200) {
                    $("#ProductCatId").val(null);

                    var r = $(".CategoryManager select").filter(function () {
                        return parseInt($(this).attr("data-level")) > 1;
                    });
                    r.remove();
                    $(".CategoryManager select option").remove();
                    var html = "<option id='0' value='Null'>هیچکدام</option>";
                    $(".CategoryManager select").append(html);
                    for (var i = 0; i < response.data.length; i++) {
                        html = "<option id='Cat" + response.data[i].Id + "' value='" + response.data[i].Id + "'>" + response.data[i].Title + "</option>";
                        $(".CategoryManager select").append(html);
                    }
                }
            }

        });
    });
    if ($(".CategoryManager #ProductCatId").val() != "") {
        var CurrentCatId = parseInt($(".CategoryManager #ProductCatId").val());
        if (CurrentCatId > 0) {
            $(".loading").show();
            $.ajax({
                type: "POST",
                url: '/Admin/Products/GetParentCategory/',
                data: '{CategoryId:"' + CurrentCatId + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.statusCode == 201) {
                        var ProductTypeId = 0;
                        if ($("#ProductTypeId").val() != undefined)
                            ProductTypeId = $("#ProductTypeId").val();
                        else
                            ProductTypeId = $("#OfferProductTypeId").val();
                        $(".CategoryManager").html("<input type='hidden' name='ProductCatId' id='ProductCatId' value='null' />");
                        for (var i = 0; i < response.data.length; i++) {
                            var id = "";
                            if (response.data[i].ParrentId != null)
                                id = response.data[i].ParrentId;
                            var html = "<select data-level='" + i + "' data-id='" + id + "' id='Category" + id + "' name='Category" + id + "' class='form-control '><option id='0' value='0'>--انتخاب نمایید--</option></select>";
                            $(".CategoryManager").append(html);
                            LoadChildCategory($(".CategoryManager select").last(), id, "option", ProductTypeId);
                        }
                        $(".CategoryManager select").each(function () {
                            $(".CategoryManager select option[value='" + $(this).attr("data-id") + "']").prop("selected", true);
                        });
                        $(".CategoryManager select option[value='" + CurrentCatId + "']").prop("selected", true);
                        $("#ProductCatId").val(CurrentCatId);
                    }
                    else if (response.statusCode == 200) {
                        $(".CategoryManager select option[value='" + response.data + "']").prop("selected", true);
                        LoadChildCategory($(".CategoryManager select").first(), response.data, "select", ProductTypeId);
                    }
                    $(".loading").hide();
                }
            });
        }
        else {
            $(".CategoryManager select option:eq(0)").prop("selected", true);
        }
    }

    $(".CategoryManager").on("change", "select", function () {
        var $this = $(this);
        if ($this.val() > 0) {
            var ProductTypeId = 0;
            if ($("#ProductTypeId").val() != undefined)
                ProductTypeId = $("#ProductTypeId").val();
            else
                ProductTypeId = $("#OfferProductTypeId").val();
            LoadChildCategory($this, $this.val(), "select", ProductTypeId);
            $(".CategoryManager #ProductCatId").val($this.val());
        }
        else {
            $this.parent().find("select").filter(function () { return $(this).attr("data-level") > (parseInt($this.attr("data-level"))); }).remove();
            $(".CategoryManager #ProductCatId").val( $this.prev().find("option:selected").val());
        }

    });
});

function LoadChildCategory($this, value, type,productTypeId) {
    value == "" ? value = null : value;
    $(".loading").show();
    $.ajax({
        async:false,
        type: "POST",
        url: '/Admin/Products/GetChildCategory/',
        data: '{CategoryId:"' + value + '",ProductTypeId:"'+productTypeId+'"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response.statusCode == 200) {
                if (type == "select")
                    $this.parent().find("select").filter(function () { return $(this).attr("data-level") > (parseInt($this.attr("data-level"))); }).remove();
                if (response.data.length > 0) {
                    var html = "";
                    if (type == "select")
                        html += "<select data-level='" + (parseInt($this.attr("data-level")) + 1) + "' id='Category" + value + "' name='Category" + value + "' class='form-control'><option id='0' value='0'>--انتخاب نمایید--</option>";
                    for (var i = 0; i < response.data.length; i++) {
                        if (type == "select")
                            html += "<option id='Cat" + response.data[i].Id + "' value='" + response.data[i].Id + "'>" + response.data[i].Title + "</option>";
                        else {
                            $this.append("<option id='Cat" + response.data[i].Id + "' value='" + response.data[i].Id + "'>" + response.data[i].Title + "</option>");
                        }
                    }
                    if (type == "select")
                        html += "</select>";
                    $this.parent().append(html);
                }

                $("#ProductCatId").val(value);
            }
            $(".loading").hide();
        }
    });

}