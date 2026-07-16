using OnlineGym.Application.Domain;

namespace OnlineGym.Application.Interfaces.Repositories;

public interface IPricingPackageRepository
{
    long Insert(PricingPackage pricingPackage);
    PricingPackage? GetById(long id);
    List<PricingPackage> GetByTrainerId(long trainerId);
    void Update(PricingPackage pricingPackage);
    void Delete(long id);
}