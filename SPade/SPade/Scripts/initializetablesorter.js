$(document).ready(function () {
    $(function () {
        $("table").tablesorter(
        {
            theme: 'bootstrap',

            widthFixed: true,
            headerTemplate: '{content} {icon}', // Add icon for various themes
            sortList: [[0, 0]],
            widgets: ['stickyHeaders', 'uitheme', 'filter'],

            widgetOptions: {
                filter_external: '#search',
                filter_columnFilters: false
            }

        });
    });
});