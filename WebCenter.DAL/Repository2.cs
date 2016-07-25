 

using WebCenter.IDAL;
using WebCenter.Entities;

namespace WebCenter.DAL
{
   
	
	public partial class memberRepository :BaseRepository<member>,ImemberRepository
    {
         
    }
	
	public partial class organizationRepository :BaseRepository<organization>,IorganizationRepository
    {
         
    }
	
	public partial class positionRepository :BaseRepository<position>,IpositionRepository
    {
         
    }
	
	public partial class roleRepository :BaseRepository<role>,IroleRepository
    {
         
    }
	
	public partial class sequenceRepository :BaseRepository<sequence>,IsequenceRepository
    {
         
    }
	
}