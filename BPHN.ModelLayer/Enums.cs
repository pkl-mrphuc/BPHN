using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.ModelLayer
{
    public enum GenderEnum
    {
        MALE = 0,
        FEMALE = 1,
        OTHER = 2
    }

    public enum RoleEnum
    {
        ADMIN = 0,
        TENANT = 1
    }

    public enum MailTypeEnum
    {
        SET_PASSWORD = 0
    }

    public enum QueueJobTypeEnum
    {
        SEND_MAIL = 0
    }
}
