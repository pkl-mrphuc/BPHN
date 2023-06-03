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

    public enum ActionEnum
    {
        LOGIN = 0,
        REGISTER_ACCOUNT = 1,
        SEND_RESET_PASSWORD = 2,
        SUBMIT_RESET_PASSWORD = 3,
        SAVE_CONFIG = 4,
        INSERT = 5
    }

    public enum ActiveStatusEnum
    {
        ACTIVE = 0,
        INACTIVE = 1
    }
}
