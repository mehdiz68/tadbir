
$(document).ready(function () {

    //UI Slider
    var skipSlider = document.getElementById('skipstep');
    var min = parseInt($("#skip-value-lower").attr("data-min"));
    var max = parseInt($("#skip-value-upper").attr("data-max"));
    var start = parseInt($("#skip-value-lower").attr("data-start"));
    var end = parseInt($("#skip-value-upper").attr("data-end"));


    noUiSlider.create(skipSlider, {
        direction: 'rtl',
        start: [start, end],
        //tooltips: [true, true],
        range: {
            'min': min,
            'max': max
        },
        step: max / 10,
        format: wNumb({
            decimals: 0,
            thousand: '،'
        })
    });
    var skipValues = [
    document.getElementById('skip-value-lower'),
    document.getElementById('skip-value-upper')
    ];
    skipSlider.noUiSlider.on('update', function (inputs, handle) {
        skipValues[handle].innerHTML = inputs[handle];
    });
    skipSlider.noUiSlider.on('change', function (inputs, handle) {
        skipValues[handle].innerHTML = inputs[handle];
        $(".startPrice input[type=hidden]").val(inputs[0].replace(/،/g, ""));
        $(".EndPrice input[type=hidden]").val(inputs[1].replace(/،/g, ""));
        $("#form-course-filter").submit();
        $("#SortAttributeId option").first().prop("selected", true);
        $("#SortCondition option").first().prop("selected", true);
        $("#PageSize option").first().prop("selected", true);
    });

    $(".filter-layer,#sortOrder,#sortOrderType").change(function () {
        $("#form-course-filter").submit();
    });

});