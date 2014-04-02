// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DteInitializer.cs" company="Company">
//   Copyrights 2014.
// </copyright>
// <summary>
//   The dte initializer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Company.PocoGenerator
{
    using System;

    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell.Interop;

    /// <summary>
    /// The dte initializer.
    /// </summary>
    public class DteInitializer : IVsShellPropertyEvents
    {
        /// <summary>
        /// The shell service.
        /// </summary>
        private IVsShell shellService;

        /// <summary>
        /// The cookie.
        /// </summary>
        private uint cookie;

        /// <summary>
        /// The callback.
        /// </summary>
        private Action callback;

        /// <summary>
        /// Initializes a new instance of the <see cref="DteInitializer"/> class.
        /// </summary>
        /// <param name="shellService">
        /// The shell service.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        internal DteInitializer(IVsShell shellService, Action callback)
        {
            int hr;

            this.shellService = shellService;
            this.callback = callback;

            // Set an event handler to detect when the IDE is fully initialized
            hr = this.shellService.AdviseShellPropertyChanges(this, out this.cookie);

            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(hr);
        }

        /// <summary>
        /// The on shell property change.
        /// </summary>
        /// <param name="propid">
        /// The property id.
        /// </param>
        /// <param name="var">
        /// The variable.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        int IVsShellPropertyEvents.OnShellPropertyChange(int propid, object var)
        {
            int hr;
            bool isZombie;

            if (propid == (int)__VSSPROPID.VSSPROPID_Zombie)
            {
                isZombie = (bool)var;

                if (!isZombie)
                {
                    // Release the event handler to detect when the IDE is fully initialized
                    hr = this.shellService.UnadviseShellPropertyChanges(this.cookie);

                    Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(hr);

                    this.cookie = 0;

                    this.callback();
                }
            }

            return VSConstants.S_OK;
        }
    }
}
