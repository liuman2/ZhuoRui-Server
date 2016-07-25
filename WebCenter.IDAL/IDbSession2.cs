 

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace WebCenter.IDAL
{
    public partial interface IDbSession
    {
   
	  

		IDAL.ImemberRepository memberRepository { get; }
	  

		IDAL.IorganizationRepository organizationRepository { get; }
	  

		IDAL.IpositionRepository positionRepository { get; }
	  

		IDAL.IroleRepository roleRepository { get; }
	  

		IDAL.IsequenceRepository sequenceRepository { get; }
	}
}