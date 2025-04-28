$(document).ready(function () {

    //Rate
    $("#ContentRate").rating({
        min: 0, max: 5, step: 0.5, size: 'xs', value: 3,
        starCaptions: function (val) {
            return val;
        },
        starCaptionClasses: function (val) {
            if (val < 3) {
                return 'label label-danger';
            } else {
                return 'label label-success';
            }
        },
        hoverOnClear: false
    }).on("rating.change", function (event, value, caption) {
        event.preventDefault();
        $(".loading").show();
        $.ajax({
            type: "POST",
            url: "/content/AddRate?ContentId=" + $("#Rate").attr("data-ContentId") + "&value=" + value,
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 200) {
                    alert(response.Message);
                    $("#RateResult span[itemprop='ratingValue']").text(response.AverageRating);
                    $("#RateResult span[itemprop='reviewCount']").text(response.TotalRaters);
                    $('#ContentRate').rating('update', response.AverageRating);

                } else {
                    alert(response.Message);
                }
                $(".loading").hide();
            },
            error: function (e) {
                alert(e);
                $(".loading").hide();

            }
        });
    });
    if ($("#RateResult") != undefined) {
        $('#ContentRate').rating('update', $("#RateResult span[itemprop='ratingValue']").text());
    }

    //Other Image
    $("a[rel^='prettyPhoto']").prettyPhoto({ slideshow: 10000, autoplay_slideshow: true, social_tools: false });

    //Rate Comment window.location.protocol + "//" +window.location.host +
    $("#comments .fa-plus-square").click(function () {
        $(".loading").show();
        var $this = $(this);
        $.ajax({
            type: "POST",
            url: "/content/PlusComment?CommentId=" + $($this).attr("data-id"),
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 200) {
                    $($this).prev().text(parseInt($($this).prev().text()) + 1);

                } else {
                    alert(response.Message);
                }
                $(".loading").hide();
            },
            error: function (e) {
                alert(e);
                $(".loading").hide();

            }
        });
    });/*"http://"window.location.protocol + "//" + window.location.host + +*/
    $("#comments .fa-minus-square").click(function () {
        $(".loading").show();
        var $this = $(this);
        $.ajax({
            type: "POST",
            url: "/content/MinusComment?CommentId=" + $($this).attr("data-id"),
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 200) {
                    $($this).prev().text(parseInt($($this).prev().text()) + 1);

                } else {
                    alert(response.Message);
                }
                $(".loading").hide();
            },
            error: function (e) {
                alert(e);
                $(".loading").hide();

            }
        });
    });

    $(".like").click(function () {
        $(".loading").show();
        var $this = $(this);
        $.ajax({
            type: "POST",
            url: "/Video/Like?Id=" + $($this).attr("data-id"),
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 200) {
                    $($this).find(".text").text(parseInt($($this).find(".text").text()) + 1);

                } else {
                    alert(response.Message);
                }
                $(".loading").hide();
            },
            error: function (e) {
                alert(e);
                $(".loading").hide();

            }
        });
    });
    $(".Dislike").click(function () {
        $(".loading").show();
        var $this = $(this);
        $.ajax({
            type: "POST",
            url: "/Video/DisLike?Id=" + $($this).attr("data-id"),
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 200) {
                    $($this).find(".text").text(parseInt($($this).find(".text").text()) + 1);

                } else {
                    alert(response.Message);
                }
                $(".loading").hide();
            },
            error: function (e) {
                alert(e);
                $(".loading").hide();

            }
        });
    });

    $(".share").click(function () {
        $("#Share").fadeToggle();
    });

});

//Comment
function NestedComment(ParentID) {
    $("#AddComment .AnswerTo").remove();
    if ($("#mLangId").val()=="1")
        $("#AddComment").prepend("<p class='AnswerTo'><span class='fa fa-remove removeAnsertToComment'></span> پاسخ به کامنتِ " + $("#Comment_" + ParentID).find(".comment-classic-name").first().text() + " </p>");
    else
        $("#AddComment").prepend("<p class='AnswerTo'><span class='fa fa-remove removeAnsertToComment'></span> Answer to : " + $("#Comment_" + ParentID).find(".comment-classic-name").first().text() + " </p>");
    $("#CommentParrentId").val(ParentID);
    $('html, body').animate({
        scrollTop: $("#AddComment").offset().top - 80
    }, 800);
    $(".removeAnsertToComment").click(function () {
        $("#CommentParrentId").val(null);
        $("#AddComment .AnswerTo").remove();
    });
}