
$(document).ready(function () {

    $(".RankItem").each(function () {
        var $this = $(this);
        $this.barrating({
            theme: 'bars-1to10',
            onSelect: function (value, text, event) {
                if (typeof (event) !== 'undefined') {
                    // rating was selected by a user
                    console.log(event.target);
                    $this.attr("data-val", value);
                    if ($this.parent().parent().find("input[name='rankvalue']").val() != undefined) {
                        $this.parent().parent().find("input[name='rankvalue']").val(value);
                    }
                    else {
                        $this.parent().parent().append("<input type='hidden' name='rankvalue' value='" + value + "' /> ");
                    }
                } else {
                }
            }
        });
    });

    //Add Advantage
    $(".advantages").on("click", ".fa-plus", function () {
        if ($(".txbAdvantage").length < 7) {
            $(".advantages").append("<input type='text' name='advantages' class='form-control txbAdvantage' /> <span class='fa fa-plus'></span> &nbsp; <span class='fa fa-minus'></span>");
        }
    });
    $(".advantages").on("click", ".fa-minus", function () {
        if ($(".txbAdvantage").length > 1) {
            $(this).prev().remove();
            $(this).prev().remove();
            $(this).remove();
        }
    });
    //Add Disadvantages
    $(".Disadvantages").on("click", ".fa-plus", function () {
        if ($(".txbDisadvantage").length < 7) {
            $(".Disadvantages").append("<input type='text' name='Disadvantages' class='form-control txbDisadvantage' /> <span class='fa fa-plus'></span> &nbsp; <span class='fa fa-minus'></span>");
        }
    });
    $(".Disadvantages").on("click", ".fa-minus", function () {
        if ($(".txbDisadvantage").length > 1) {
            $(this).prev().remove();
            $(this).prev().remove();
            $(this).remove();
        }
    });


    $("#AddComment input,#AddComment textarea").click(function () {
        $("#RankValues").html("");
        $(".Rank input[type='hidden']").each(function () {
            $("#RankValues").append($(this).clone());
        });
    });

    if ($(".alert-add").length) {
        swal({ title: "", text: $(".alert-add").text(), type: "success", showCancelButton: false, confirmButtonClass: 'btn-success', confirmButtonText: 'بستن', timer: 3000 });

    }

    $(".btn-info").click(function () {
        $(".loading").show();
    });




    $("#OrderItems button[type='submit']").click(function (e) {
        $(".loading").show();
        $("#OrderItems input[name='RateItem']").val($(this).attr("data-id"));
    });
});