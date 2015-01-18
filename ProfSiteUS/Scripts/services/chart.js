'use strict';

angular.module('chartsApp')
.factory('chartDataService', ['$q', '$resource', function ($q, $resource) {
    var chartData = {
        fipronilMethopreneEfficacy: {
            title: 'Fipronil(s) methoprene efficacy from baseline to day 60<sup>2</sup>',
            yAxis: {
                labels: [
                    100,
                    80,
                    60,
                    40,
                    20
                ],
                title: '%'
            },
            xAxis: {
                title: 'Day',
                cssClass: 'sixSeries',
                dataPoints: [
                    {
                        name: '7',
                        color: 'green',
                        data: [
                            85.5
                        ]
                    },
                    {
                        name: '14',
                        color: 'green',
                        data: [
                            77.3
                        ]
                    },
                    {
                        name: '21',
                        color: 'green',
                        data: [
                            51.5
                        ]
                    },
                    {
                        name: '28-30',
                        color: 'green',
                        data: [
                            49.5
                        ]
                    },
                    {
                        name: '40-45',
                        color: 'green',
                        data: [
                            71.6
                        ]
                    },
                    {
                        name: '54-60',
                        color: 'grey',
                        data: [
                            54.8
                        ]
                    }
                ]
            }
        },
        speedKill: {
            title: 'Flea "speed of kill" results',
            yAxis: {
                labels: [
                    100,
                    80,
                    60,
                    40,
                    20
                ],
                title: '%'
            },
            xAxis: {
                title: '',
                cssClass: 'fourSeries',
                dataPoints: [
                    {
                        name: '0',
                        data: [
                            {
                                cssClass: 'blue',
                                'value': 80.5
                            },
                            {
                                cssClass: 'grey',
                                'value': 99.4
                            },
                            {
                                cssClass: 'purple',
                                'value': 100
                            },
                            {
                                cssClass: 'red',
                                'value': 100
                            }
                        ]
                    },
                    {
                        name: '28',
                        color: 'grey',
                        data: [
                            {
                                cssClass: 'blue',
                                'value': 96.8
                            },
                            {
                                cssClass: 'grey',
                                'value': 100
                            },
                            {
                                cssClass: 'purple',
                                'value': 100
                            },
                            {
                                cssClass: 'red',
                                'value': 100
                            }
                        ]
                    },
                    {
                        name: '56',
                        color: 'purple',
                        data: [
                            {
                                cssClass: 'blue',
                                'value': 91.4
                            },
                            {
                                cssClass: 'grey',
                                'value': 100
                            },
                            {
                                cssClass: 'purple',
                                'value': 99.8
                            },
                            {
                                cssClass: 'red',
                                'value': 100
                            }
                        ]
                    },
                    {
                        name: '84',
                        color: 'red',
                        data: [
                            {
                                cssClass: 'blue',
                                'value': 33.5
                            },
                            {
                                cssClass: 'grey',
                                'value': 98
                            },
                            {
                                cssClass: 'purple',
                                'value': 98.7
                            },
                            {
                                cssClass: 'red',
                                'value': 100
                            }
                        ]
                    }
                ]
            },
            legend: {
                dataPoints: [
                    {
                        cssClass: 'blue',
                        'label': '4h'
                    },
                    {
                        cssClass: 'grey',
                        'label': '8h'
                    },
                    {
                        cssClass: 'purple',
                        'label': '12h'
                    },
                    {
                        cssClass: 'red',
                        'label': '24h'
                    }
                ]
            }
        },
        householdsFreeFleas: {
            title: 'Percentage of households free of fleas<sup>7</sup>',
            yAxis: {
                labels: [
                    100,
                    80,
                    60,
                    40,
                    20
                ],
                title: '%'
            },
            xAxis: {
                title: '',
                cssClass: 'doubleSeries',
                dataPoints: [
                    {
                        name: 'Week 2',
                        data: [
                            {
                                cssClass: 'blue',
                                'value': 89.57
                            },
                            {
                                cssClass: 'grey',
                                'value': 62.3
                            }
                        ]
                    },
                    {
                        name: 'Week 4',
                        data: [
                            {
                                cssClass: 'blue',
                                'value': 94.87
                            },
                            {
                                cssClass: 'grey',
                                'value': 63.93
                            }
                        ]
                    },
                    {
                        name: 'Week 8',
                        data: [
                            {
                                cssClass: 'blue',
                                'value': 95.65
                            },
                            {
                                cssClass: 'grey',
                                'value': 70.49
                            }
                        ]
                    },
                    {
                        name: 'Week 12',
                        data: [
                            {
                                cssClass: 'blue',
                                'value': 97.39
                            },
                            {
                                cssClass: 'grey',
                                'value': 81.97
                            }
                        ]
                    }
                ]
            },
            legend: {
                cssClass: 'wide',
                dataPoints: [
                    {
                        cssClass: 'blue',
                        'label': 'Bravecto<sup>&trade;</sup> (fluralaner)'
                    },
                    {
                        cssClass: 'grey',
                        'label': 'Frontline<sup>&reg;</sup> (fipronil)'
                    }
                ]
            }
        },
        tickControlEfficacy: {
            title: 'Tick control efficacy of Bravecto<sup>7</sup>',
            yAxis: {
                labels: [
                    100,
                    80,
                    60,
                    40,
                    20
                ],
                title: '%'
            },
            xAxis: {
                cssClass: 'singleSeries singleColor fat',
                dataPoints: [
                    {
                        name: 'Week 2',
                        cssClass: 'fat',
                        data: [
                            99.9
                        ]
                    },
                    {
                        name: 'Week 4',
                        cssClass: 'fat',
                        data: [
                            99.9
                        ]
                    },
                    {
                        name: 'Week 8',
                        cssClass: 'fat',
                        data: [
                            99.7
                        ]
                    },
                    {
                        name: 'Week 12',
                        cssClass: 'fat',
                        data: [
                            100
                        ]
                    }
                ]
            }
        },
        petOwnership: {
            title: 'Pet Ownership Among U.S. Internet Users, April 2013<sup>22</sup>',
            yAxis: {
                labels: [
                    100,
                    80,
                    60,
                    40,
                    20
                ],
                title: '%'
            },
            xAxis: {
                title: '',
                cssClass: 'singleSeries singleColor fat',
                dataPoints: [
                    {
                        name: 'Dogs',
                        cssClass: 'fat',
                        data: [
                            48
                        ]
                    },
                    {
                        name: 'Cats',
                        cssClass: 'fat',
                        data: [
                            34
                        ]
                    },
                    {
                        name: 'Any pet',
                        cssClass: 'fat',
                        data: [
                            67
                        ]
                    },
                    {
                        name: 'Other',
                        cssClass: 'fat',
                        data: [
                            12
                        ]
                    }
                ]
            }
        }
    }

    return {
        getDataById: function(id)
        {
            var deferred = $q.defer();

            deferred.resolve(chartData[id]);

            return deferred.promise;
        }
    };
}]);