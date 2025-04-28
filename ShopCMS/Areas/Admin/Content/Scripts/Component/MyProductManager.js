function CreateProducts(container) {
    //$('html, body').animate({
    //    scrollTop: $(".main-header").offset().top
    //}, 500);
    //Category js load again

    
    $("body").on("change","#ProductTypeId2",function () {
        var typeid = $("#ProductTypeId2").val();
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
                        $(".CategoryManager").html("<input type='hidden' name='ProductCatId' id='ProductCatId' value='null' />");
                        for (var i = 0; i < response.data.length; i++) {
                            var id = "";
                            if (response.data[i].ParrentId != null)
                                id = response.data[i].ParrentId;
                            var html = "<select data-level='" + i + "' data-id='" + id + "' id='Category" + id + "' name='Category" + id + "' class='form-control '><option id='0' value='0'>--انتخاب نمایید--</option></select>";
                            $(".CategoryManager").append(html);
                            LoadChildCategory($(".CategoryManager select").last(), id, "option",$("#ProductTypeId2").val());
                        }
                        $(".CategoryManager select").each(function () {
                            $(".CategoryManager select option[value='" + $(this).attr("data-id") + "']").prop("selected", true);
                        });
                        $(".CategoryManager select option[value='" + CurrentCatId + "']").prop("selected", true);
                        $("#ProductCatId").val(CurrentCatId);
                    }
                    else if (response.statusCode == 200) {
                        $(".CategoryManager select option[value='" + response.data + "']").prop("selected", true);
                        LoadChildCategory($(".CategoryManager select").first(), response.data, "select",$("#ProductTypeId2").val());
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
            LoadChildCategory($this, $this.val(), "select", $("#ProductTypeId2").val());
            $(".CategoryManager #ProductCatId").val($this.val());
        }
        else {
            $this.parent().find("select").filter(function () { return $(this).attr("data-level") > (parseInt($this.attr("data-level"))); }).remove();
            $(".CategoryManager #ProductCatId").val($this.prev().find("option:selected").val());
        }

    });

    $(".ajax-form-manager").attr("data-ajax-success", "$('.loading').hide();CreateProducts('" + container + "')");
    Init_Products(container);
}
function Init_Products(container) {
   

    $("#ProductManagerPagination .pagination li a").click(function (e) {
        e.preventDefault();
        var PostUrl = $(this).attr("href");
        $('.loading').show();
        $.ajax({
            type: 'GET',
            cache: false,
            async: true,
            url: PostUrl,
            UpdateTargetId: "ProductView",
            success: function (html) {
                $('.loading').hide();
                $('#ProductView').empty();
                $('#ProductView').html(html);
                Init_Products(container);
            },
            error: function () { $('.loading').hide(); alert("error"); }
        });
    });

    $(".AddForLinkProduct").click(function () {
        var $this = $(this);
        $("." + container).html("<p class='text text-success' data-id='" + $this.attr("data-Id") + "'> <span class='glyphicon glyphicon-remove' style='color:red;cursor:pointer'></span> " + $this.attr("data-Title") + "</p>");
        Cancel_btn_handler_ProductManager();
    });

    $(".LinkProduct").on("click", ".glyphicon-remove", function () {
        $(this).parent().remove();
    });

    $("#ProductManager button[id=Cancel_btn_ProductManager]").click(function () {
        Cancel_btn_handler_ProductManager()
    });
   
    $("#SearchPageShow").click(function () {
        $("#SearchPage").slideToggle();
        $("#SearchPageShow").toggleClass("hilightSearch");
    });
    $.blockUI.defaults.overlayCSS = {
        backgroundColor: '#000',
        opacity: 0.6
    };
    $.blockUI.defaults.css = {
        padding: 0,
        margin: 5,
        width: '98%',
        top: '1%',
        left: '1%',
        color: '#000',
        border: '3px solid #aaa',
        backgroundColor: '#fff'
    };
    $.blockUI({ message: $('#ProductView') });

    $("#Cancel_btn_SearchProductManager").click(function () {
        $("#SearchPage").slideUp();
        $("#SearchPageShow").removeClass("hilightSearch");
    });

}

function Cancel_btn_handler_ProductManager() {
    $('#ProductView').empty();
    $.unblockUI();
}

$(document).on('keydown', function (e) {
    if (e.keyCode === 27) { // ESC
        Cancel_btn_handler_ProductManager();
    }
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