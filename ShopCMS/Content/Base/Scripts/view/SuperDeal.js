$(document).ready(function () {
    $(document).ready(function () {
        $('.sc-wrapper').dragscrollable({
            dragSelector: 'li',
            acceptPropagatedEvent: false
        });
    });
    $(".Load-More").click(function () {
        $(".loading").show();
        var t = $(this).offset().top;
        var $this = $(this);
        var pids = [];
        $this.parent().parent().find(".SuperDealProducts input").each(function () {
            pids.push($(this).val());
        });
        $.ajax({
            type: "POST",
            url: "/SuperDeal/LoadMorePr",
            data: JSON.stringify({ id: $this.attr("data-id"), pagenumber: $this.attr("data-pagenumber"), pids: pids }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {

                if (response.statusCode == 200) {
                    $this.parent().parent().find(".SuperDealProducts").append(response.NewRow);
                    $('html, body').animate({
                        scrollTop: t + 40
                    }, 600);
                }
                else if (response.statusCode == 404) {
                    window.location.href = "/";
                }
                else {
                    alert(response.message);
                }


                $('.product_color_switch span').each(function () {
                    var get_color = $(this).attr('data-color');
                    $(this).css("background-color", get_color);
                });

                $this.attr("data-pagenumber", parseInt($this.attr("data-pagenumber")) + 1);

                if (response.finish == true) {
                    $this.remove();
                }
                $(".loading").hide();
            }

        });
    });
});
