$(document).ready(function() {

    "use strict";

    // alert(bvLanguage);

    var options = {
        events_source: '/sharesocial/socialCalendarPosts',
        view: 'month',
        tmpl_path: '/Content/calendartemplate/',
        tmpl_cache: false,
        // day: '2013-03-12',
        // modal: '#events-modal',
        // language: bvLanguage, TODO: Uncomment for localization
        time_start: '00:30',
        time_end: '23:30',
        language: bvLanguage,
        onAfterEventsLoad: function (events) {
            //alert("onAfterEventsLoad");
            //if (!events) {
            //    return;
            //}
            //var list = $('#eventlist');
            //list.html('');

            //$.each(events, function (key, val) {
            //    $(document.createElement('li'))
            //        .html('<a href="' + val.url + '">' + val.title + '</a>')
            //        .addClass(val.network)
            //        .appendTo(list);
            //});            
        },
        onBeforeEventsLoad: function (next) {
            //alert("onAfterViewLoad");
            next();
        },
        onAfterViewLoad: function (view) {
            //alert("onAfterViewLoad");
            $('.calendar-title').text(this.getTitle());
            $('.btn-group button').removeClass('active');
            $('button[data-calendar-view="' + view + '"]').addClass('active');
            if ($('.day-borders').length > 0) {
                $('.day-borders').height($('.cal-week-box').height() - 2);
            }
        },
        classes: {
            months: {
                general: 'label'
            }
        }
    };

    var calendar = $('#socialCalendar').calendar(options);
	calendar.setLanguage(bvLanguage);

    $('.btn-group button[data-calendar-nav]').each(function () {
        var $this = $(this);
        $this.click(function () {
            calendar.navigate($this.data('calendar-nav'));
        });
    });

    $('.btn-group button[data-calendar-view]').each(function () {
        var $this = $(this);
        $this.click(function () {
            calendar.view($this.data('calendar-view'));
        });
    });


    // set up click events

    //$(".event-twitter").socialPostDialog({ "dialogType": "socialcal" });
    //$(".event-facebook").socialPostDialog({ "dialogType": "socialcal" });
  

    //$('.cal-month-day').click(function () {
    //    //$(".event-item").socialPostDialog({ "dialogType": "socialcal" });
    //    //$(".event-twitter").socialPostDialog({ "dialogType": "socialcal" });
    //    //$(".event-facebook").socialPostDialog({ "dialogType": "socialcal" });
    //});

});