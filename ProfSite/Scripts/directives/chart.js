'use strict';

angular.module('chartsApp')
.directive('bravoChart', ['chartDataService', function(chartDataService)
{
    return {
        scope: {},
        restrict: 'A',
        template: '<div class="chart">\
                        <div class="chart-header">\
                            <div class="icon"></div>\
                            <h3 ng-bind-html="chartData.title"></h3>\
                        </div>\
                        <div class="yAxis" ng-if="chartData.yAxis != null">\
                            <div class="labels">\
                                <div class="label" ng-repeat="label in chartData.yAxis.labels">{{label}}</div>\
                            </div>\
                            <div class="title" ng-bind-html="chartData.yAxis.title"></div>\
                        </div>\
                        <div class="chart-body">\
                            <div class="series {{chartData.xAxis.cssClass}}" ng-repeat="item in chartData.xAxis.dataPoints">\
                                <div ng-if="item.data.length == 1" class="column {{item.cssClass}}" ng-repeat="bar in item.data track by $index" ng-style="getBarHeight(bar)"></div>\
                                <div ng-if="item.data.length >= 2" class="column {{bar.cssClass}}" ng-repeat="bar in item.data" ng-style="getBarHeight(bar.value)"></div>\
                            </div>\
                            <div class="chart-footer">\
                                <div class="category" ng-repeat="category in chartData.xAxis.dataPoints track by $index" ng-style="getWidth(chartData.xAxis.dataPoints.length)" ng-bind-html="category.name"></div>\
                                <div class="xAxis" ng-if="chartData.xAxis != null">\
                                    <div class="title" ng-bind-html="chartData.xAxis.title"></div>\
                                </div>\
                            </div>\
                        </div>\
                        <div class="legend {{chartData.legend.cssClass}}" ng-if="chartData.legend != null">\
                            <div class="legendItem" ng-repeat="item in chartData.legend.dataPoints">\
                                <div class="{{item.cssClass}}">\
                                    <span class="colorSwatch {{item.cssClass}}"></span>\
                                    <span ng-bind-html="item.label"></span>\
                                </div>\
                            </div>\
                        </div>\
                    </div>',
        replace: true,
        link: function($scope, elm, attrs)
        {
            chartDataService.getDataById(attrs.chartId).then(function(d)
            {
                $scope.chartData = d;
            });

            $scope.getBarHeight = function(val)
            {
                return {height: ((val/100) * 145) + 'px'};
            }

            $scope.getWidth = function(val)
            {
                return {width: (100/val) + '%'};
            }
        }
    };
}]);