 
 
using WebCenter.IDAL;

namespace WebCenter.DAL
{
    public partial class DbSession:IDbSession  
    {   
	
	public IDAL.IareaRepository areaRepository { get { return new areaRepository(); } }
	
	public IDAL.ImemberRepository memberRepository { get { return new memberRepository(); } }
	
	public IDAL.IorganizationRepository organizationRepository { get { return new organizationRepository(); } }
	
	public IDAL.IpositionRepository positionRepository { get { return new positionRepository(); } }
	
	public IDAL.IroleRepository roleRepository { get { return new roleRepository(); } }
	
	public IDAL.IsequenceRepository sequenceRepository { get { return new sequenceRepository(); } }
	}
}