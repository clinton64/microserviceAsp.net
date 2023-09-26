using AutoMapper;
using Discount.Grpc.Entities;
using Discount.Grpc.Protos;

namespace Discount.Grpc.Mapper
{
	public class DsicountProfile : Profile
	{
		public DsicountProfile() 
		{
			CreateMap<Coupon, CouponModel>().ReverseMap();
		}
	}
}
