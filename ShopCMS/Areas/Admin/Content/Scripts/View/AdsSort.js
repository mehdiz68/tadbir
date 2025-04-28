$(document).ready(function () {

    $("#TypeId,#LinkIdIdSelectListItem,#PositionId").change(function () {
        Search();
    });
    $(".tab-content .tab-pane").each(function () {
        var $this = $(this);

        //---------------------------------
        //Sort
        $this.find(".table").sortable({
            items: ".rowItem",
            cursor: 'move',
            opacity: 0.6,
            placeholder: "ui-state-highlight",
            update: function () {
                sendPageOrderToServer();
            }
        });
        function sendPageOrderToServer() {
            var order = $this.find(".table").sortable("toArray");
            $.ajax({
                crossDomain: true,
                type: "POST",
                url: "/Adveresting/Sort",
                data: '{ids:"' + order + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.statusCode == 400) {
                        $(".table-responsive").before("<p style='color:#fff;background-color:green;padding:5px;text-align:right' class='catmsg'> انجام شد . </p>").fadeIn(300);
                        $(".catmsg").delay(1000).fadeOut(300, function () { $(this).remove(); });
                    } else {
                        $(".table-responsive").before("<p style='color:#fff;background-color:red;padding:5px;text-align:right' class='catmsg'> خطا </p>").fadeIn(300);;
                        $(".catmsg").delay(1000).fadeOut(300, function () { $(this).remove(); });
                    }
                }
            });
        }
    })

});

function Search()
{
    var LinkId = null;
    var TypeId = null;
    var PositionId = null;
    if($("#TypeId").val())
    {
        TypeId = $("#TypeId").val();
    }
    if ($("#PositionId").val()) {
        PositionId = $("#PositionId").val();
    }
    if ($("#LinkIdIdSelectListItem").val()) {
        LinkId = $("#LinkIdIdSelectListItem").val();
    }

    if (LinkId != null && TypeId != null && PositionId!=null)
        window.location.href = "/Admin/adveresting/Sort?TypeId=" + TypeId + "&LinkId=" + LinkId + "&PositionId=" + PositionId;
    else if (LinkId != null && TypeId != null)
        window.location.href = "/Admin/adveresting/Sort?TypeId=" + TypeId + "&LinkId=" + LinkId;
    else if (PositionId != null && TypeId != null)
        window.location.href = "/Admin/adveresting/Sort?TypeId=" + TypeId + "&PositionId=" + PositionId;
    else if (TypeId != null)
        window.location.href = "/Admin/adveresting/Sort?TypeId=" + TypeId;
    else if (LinkId != null)
        window.location.href = "/Admin/adveresting/Sort?LinkId=" + LinkId;
    else if (PositionId != null)
        window.location.href = "/Admin/adveresting/Sort?PositionId=" + PositionId;
    else
            window.location.href = "/Admin/adveresting/Sort";
}