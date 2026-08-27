using RestaurantService.Domain.Entities;

namespace RestaurantService.Tests.Entities;

public class RestaurantTests
{
    private static Restaurant CreateRestaurantWithWednesdayHours(TimeSpan openTime, TimeSpan closeTime, bool isClosed = false)
    {
        var restaurant = new Restaurant();

        restaurant.OpeningHours.Add(new RestaurantOpeningHours
        {
            DayOfWeek = DayOfWeek.Wednesday,
            OpenTime = openTime,
            CloseTime = closeTime,
            IsClosed = isClosed
        });

        return restaurant;
    }

    [Fact]
    public void IsOpenNow_WhenWithinOpeningHours_ReturnsTrue()
    {
        var restaurant = CreateRestaurantWithWednesdayHours(TimeSpan.FromHours(10), TimeSpan.FromHours(22));
        var referenceTime = new DateTime(2026, 8, 26, 14, 30, 0); // sreda 14:30

        var result = restaurant.IsOpenNow(referenceTime);

        Assert.True(result);
    }

    [Fact]
    public void IsOpenNow_WhenBeforeOpeningTime_ReturnsFalse()
    {
        var restaurant = CreateRestaurantWithWednesdayHours(TimeSpan.FromHours(10), TimeSpan.FromHours(22));
        var referenceTime = new DateTime(2026, 8, 26, 9, 0, 0); // sreda 09:00, pre otvaranja

        var result = restaurant.IsOpenNow(referenceTime);

        Assert.False(result);
    }

    [Fact]
    public void IsOpenNow_WhenAfterClosingTime_ReturnsFalse()
    {
        var restaurant = CreateRestaurantWithWednesdayHours(TimeSpan.FromHours(10), TimeSpan.FromHours(22));
        var referenceTime = new DateTime(2026, 8, 26, 23, 0, 0); // sreda 23:00, posle zatvaranja

        var result = restaurant.IsOpenNow(referenceTime);

        Assert.False(result);
    }

    [Fact]
    public void IsOpenNow_WhenNoOpeningHoursForDay_ReturnsFalse()
    {
        var restaurant = CreateRestaurantWithWednesdayHours(TimeSpan.FromHours(10), TimeSpan.FromHours(22));
        var referenceTime = new DateTime(2026, 8, 27, 14, 30, 0); // cetvrtak - nema radnog vremena definisano

        var result = restaurant.IsOpenNow(referenceTime);

        Assert.False(result);
    }

    [Fact]
    public void IsOpenNow_WhenDayMarkedClosed_ReturnsFalse()
    {
        var restaurant = CreateRestaurantWithWednesdayHours(TimeSpan.FromHours(10), TimeSpan.FromHours(22), isClosed: true);
        var referenceTime = new DateTime(2026, 8, 26, 14, 30, 0); // sreda, ali oznaceno kao zatvoreno (npr. praznik)

        var result = restaurant.IsOpenNow(referenceTime);

        Assert.False(result);
    }

    [Fact]
    public void IsOpenNow_WhenExactlyAtOpeningTime_ReturnsTrue()
    {
        var restaurant = CreateRestaurantWithWednesdayHours(TimeSpan.FromHours(10), TimeSpan.FromHours(22));
        var referenceTime = new DateTime(2026, 8, 26, 10, 0, 0); // sreda tacno 10:00, trenutak otvaranja

        var result = restaurant.IsOpenNow(referenceTime);

        Assert.True(result);
    }

    [Fact]
    public void IsOpenNow_WhenExactlyAtClosingTime_ReturnsFalse()
    {
        var restaurant = CreateRestaurantWithWednesdayHours(TimeSpan.FromHours(10), TimeSpan.FromHours(22));
        var referenceTime = new DateTime(2026, 8, 26, 22, 0, 0); // sreda tacno 22:00, trenutak zatvaranja

        var result = restaurant.IsOpenNow(referenceTime);

        Assert.False(result);
    }

    [Fact]
    public void IsOpenNow_WhenHolidayExceptionMarkedClosed_ReturnsFalse()
    {
        var restaurant = CreateRestaurantWithWednesdayHours(TimeSpan.FromHours(10), TimeSpan.FromHours(22));
        var holidayDate = new DateOnly(2026, 1, 7); // Bozic, sreda

        restaurant.HolidayExceptions.Add(new RestaurantHolidayException
        {
            Date = holidayDate,
            IsClosed = true,
            Reason = "Bozic"
        });

        var referenceTime = new DateTime(2026, 1, 7, 14, 30, 0); // isto vreme kad bi inace bilo otvoreno

        var result = restaurant.IsOpenNow(referenceTime);

        Assert.False(result);
    }

    [Fact]
    public void IsOpenNow_WhenHolidayExceptionHasShortenedHours_UsesHolidayHoursInsteadOfWeeklyHours()
    {
        var restaurant = CreateRestaurantWithWednesdayHours(TimeSpan.FromHours(10), TimeSpan.FromHours(22));
        var holidayDate = new DateOnly(2026, 1, 7); // npr. Badnji dan, skraceno

        restaurant.HolidayExceptions.Add(new RestaurantHolidayException
        {
            Date = holidayDate,
            IsClosed = false,
            OpenTime = TimeSpan.FromHours(10),
            CloseTime = TimeSpan.FromHours(15)
        });

        var withinHolidayHours = restaurant.IsOpenNow(new DateTime(2026, 1, 7, 14, 0, 0));
        var afterHolidayHoursButWithinNormalHours = restaurant.IsOpenNow(new DateTime(2026, 1, 7, 16, 0, 0));

        Assert.True(withinHolidayHours);
        Assert.False(afterHolidayHoursButWithinNormalHours);
    }

    [Fact]
    public void IsOpenNow_WhenHolidayExceptionExistsForDifferentDate_FallsBackToWeeklyHours()
    {
        var restaurant = CreateRestaurantWithWednesdayHours(TimeSpan.FromHours(10), TimeSpan.FromHours(22));

        restaurant.HolidayExceptions.Add(new RestaurantHolidayException
        {
            Date = new DateOnly(2026, 1, 7),
            IsClosed = true,
            Reason = "Bozic"
        });

        var referenceTime = new DateTime(2026, 1, 14, 14, 30, 0); // naredna sreda, bez izuzetka

        var result = restaurant.IsOpenNow(referenceTime);

        Assert.True(result);
    }
}