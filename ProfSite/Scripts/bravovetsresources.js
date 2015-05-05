(function ($) {
    $.extend($.ui, { BravovetsResources: {} });

    function BravovetsResources() {
        this.widget = null;
        this.widgetList = null;
        this.filter = null;
        this.getUrl = null;
    }

    $.extend(BravovetsResources.prototype, {
        init: function (options, control) {
            var self = this;
            this.widget = control;

            var defaults = {

            };
            var allOptions = $.extend(defaults, options);

            this.getUrl = allOptions.getUrl;
            this.filters = self.widget.find(allOptions.filters);
            this.loadMore = $(allOptions.loadMore);
            this.widgetList = self.widget.find(allOptions.listName);
            this.page = 1;

            self._bindPaging();
            self._bindFilter();
            self._getResources();
        },
        _bindFilter: function () {
            var self = this;

            self.filters.on('change', function (e) {
                self.page = 1;
                self._getResources(true);
            });
        },
        _bindPaging: function () {
            var self = this;

            self.loadMore.on('click', function (e) {
                e.preventDefault();
                self.page += 1;
                self._getResources();
            });
        },
        _getResources: function (clearContent) {
            var self = this, selectedFilters = [];
            $.each(self.filters, function (i, item) {
                if ($(this).is(':checked')) {
                    selectedFilters.push($(this).val());
                }
            });

            self.loadMore.attr('disabled', true);
            $.ajax({
                url: self.getUrl,
                data: { page: self.page, filters: selectedFilters },
                traditional: true,
                type: "GET",
                cache: false,
                success: function (data) {
                    var response = $("<div>" + data + "</div");
                    var newItems = response.find("div.resource-item-row");

                    if (clearContent && clearContent === true) {
                        self.widgetList.html(newItems);
                    } else {
                        self.widgetList.append(newItems);
                    }

                    if (newItems.length === 0) {
                        self.loadMore.attr('disabled', true);
                    } else {
                        self.loadMore.removeAttr('disabled', true);
                    }
                }
            });
        }
    });

    $.fn.resources = function (options) {
        var control = new BravovetsResources();
        control.init(options, this);
        return control;
    };
})(jQuery);