using HotelViewer.Domain.Entity;
using HotelViewer.Domain.Repository;
using HotelViewer.Domain.Value;

namespace HotelViewer.ApplicationLayer.Services;

public class DbSeeder(
    IUserRepository userRepo,
    IOrganizationRepository orgRepo,
    IHotelRepository hotelRepo,
    IRoomRepository roomRepo,
    IResidentRepository residentRepo,
    IResidenceRepository residenceRepo) {
  public void Seed() {
    var adminUsername = new Username("admin");
    userRepo.FindOneById(adminUsername).IfLeft(() => {
      var admin = new User(adminUsername, Array.Empty<byte>(), Array.Empty<byte>(), UserRole.Admin);
      admin.HashNewPassword("admin");
      userRepo.Save(admin);
      return admin;
    });

    var orgId = new OrganizationId(1);
    orgRepo.FindOneById(orgId).IfLeft(() => {
      var org = new Organization(orgId, "Гранд Отель Групп");
      orgRepo.Save(org);
      return org;
    });

    var hotelId = new HotelId(1);
    hotelRepo.FindOneById(hotelId).IfLeft(() => {
      var hotel = new Hotel(
      hotelId,
      "Отель 'Звезда'",
      new PhoneNumber("88005553535"),
      new Address("г. Москва, ул. Тверская, 1"),
      orgId);
      hotelRepo.Save(hotel);
      return hotel;
    });

    var room101 = new RoomId(new RoomNumber(1, 1), hotelId);
    roomRepo.FindOneById(room101).IfLeft(() => {
      var room = new Room(room101, RoomType.Standard);
      roomRepo.Save(room);
      return room;
    });

    var room102 = new RoomId(new RoomNumber(1, 2), hotelId);
    roomRepo.FindOneById(room102).IfLeft(() => {
      var room = new Room(room102, RoomType.Standard);
      roomRepo.Save(room);
      return room;
    });

    var residentId = new ResidentId(1);
    residentRepo.FindOneById(residentId).IfLeft(() => {
      var resident = new Resident(
        residentId,
        new FullName("Иванов", "Иван", "Иванович"),
        new Address("г. Санкт-Петербург, пр. Ленина, 10"),
        Sex.Male,
        new PhoneNumber("89991234567")
      );
      residentRepo.Save(resident);
      return resident;
    });

    var residenceId = new ResidenceId(1);
    residenceRepo.FindOneById(residenceId).IfLeft(() => {
      var residence = new Residence(
        residenceId,
        room101.Number,
        hotelId,
        residentId,
        5, // 5 ночей
        DateTime.Now.AddDays(-2) // Заехал 2 дня назад
      );
      residenceRepo.Save(residence);
      return residence;
    });
  }
}
