﻿using NSBDomain.Commands;
using NSBDomain.Domain;
using SeekU.Commanding;

namespace NSBDomain.CommandHandlers
{
    public class AccountHandler :
        IHandleCommands<CreateNewAccountCommand>,
        IHandleCommands<DebitAccountCommand>,
        IHandleCommands<CreditAccountCommand>
    {
        public void Handle(CommandContext context, CreateNewAccountCommand command)
        {
            var account = new BankAccount(command.Id, command.StartingBalance);

            // This is basically a unit of work.  Persist the event stream.
            context.Finalize(account);
        }

        public void Handle(CommandContext context, DebitAccountCommand command)
        {
            var account = context.GetById<BankAccount>(command.Id);
            account.DebitAccount(command.Amount);

            context.Finalize(account);
        }

        public void Handle(CommandContext context, CreditAccountCommand command)
        {
            var account = context.GetById<BankAccount>(command.Id);
            account.CreditAccount(command.Amount);

            context.Finalize(account);
        }
    }

    //public class NewAccountCommandHandler : CommandHandler<CreateNewAccountCommand>
    //{
    //    public override void Handle(CommandContext context, CreateNewAccountCommand command)
    //    {
    //        var account = new BankAccount(command.Id, command.StartingBalance);

    //        // This is basically a unit of work.  Persist the event stream.
    //        context.Finalize(account);
    //    }
    //}

    //public class DebitAccountCommandHandler : CommandHandler<DebitAccountCommand>
    //{
    //    public override void Handle(CommandContext context, DebitAccountCommand command)
    //    {
    //        var account = context.GetById<BankAccount>(command.Id);
    //        account.DebitAccount(command.Amount);

    //        context.Finalize(account);
    //    }
    //}

    //public class CreditAccountCommandHandler : CommandHandler<CreditAccountCommand>
    //{
    //    public override void Handle(CommandContext context, CreditAccountCommand command)
    //    {
    //        var account = context.GetById<BankAccount>(command.Id);
    //        account.CreditAccount(command.Amount);

    //        context.Finalize(account);
    //    }
    //}
}
