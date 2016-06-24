
$(function () {
    var words = ['Complete task', 'Upload solution', 'Get grades'],
        index = -1,
        $el = $('#changingtext')
    setInterval(function () {
        index++ < words.length - 1 || (index = 0);
        $el.fadeOut(function () {
            $el.text(words[index]).fadeIn();
        });
    }, 2500);
});