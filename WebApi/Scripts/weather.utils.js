weatherAggregator.utils = {
    recentDays : ["Сегодня", "Завтра"],
    daysOfWeek: ['Воскресенье', 'Понедельник', 'Вторник', 'Среда', 'Четверг', 'Пятница', 'Суббота'],
    precipitation: ['Без осадков', 'Дождь', 'Снег', 'Град'],
    cloudIcons: ['Sun.png', 'SunCloud.png', 'Cloud.png'],
    cloudTitles: ['Солнечно', 'Переменная облачность', 'Пасмурно'],

    getDay: function (index) {
        if (index < 2) {
            return this.recentDays[index];
        }
        var currentDayIndex = (new Date()).getDay();
        return this.daysOfWeek[(currentDayIndex + index) % 7];
    },

    getPrecipitation: function(index) {
        return this.precipitation[index];
    },

    showWaiter: function () {
        window.waiter.show({ targetId: 'body' });
    },
    hideWaiter: function () {
        window.waiter.hide({ targetId: 'body' });
    },

    getCloudInfo: function (cloudness) {
        var index = parseInt(this.cloudIcons.length * cloudness / 100, 10);
        return {
            icon: this.cloudIcons[index],
            title: this.cloudTitles[index],
            percentage: cloudness
        };
    }
};