/* 
 *  Morgan Stanley makes this available to you under the Apache License,
 *  Version 2.0 (the "License"). You may obtain a copy of the License at
 *       http://www.apache.org/licenses/LICENSE-2.0.
 *  See the NOTICE file distributed with this work for additional information
 *  regarding copyright ownership. Unless required by applicable law or agreed
 *  to in writing, software distributed under the License is distributed on an
 *  "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
 *  or implied. See the License for the specific language governing permissions
 *  and limitations under the License.
 *  
 */

import {
    AppIdentifier,
    AppIntent,
    AppMetadata,
    Channel,
    ChannelError,
    Context,
    ContextHandler,
    DesktopAgent,
    ImplementationMetadata,
    IntentHandler,
    IntentResolution,
    Listener,
    PrivateChannel,
    ResolveError
} from '@finos/fdc3';

import { MessageRouter } from '@morgan-stanley/composeui-messaging-client';
import { ChannelType } from './infrastructure/ChannelType';
import { ComposeUIContextListener } from './infrastructure/ComposeUIContextListener';
import { ComposeUITopic } from './infrastructure/ComposeUITopic';
import { ComposeUIIntentListener } from './infrastructure/ComposeUIIntentListener';
import { Fdc3RaiseIntentRequest } from './infrastructure/messages/Fdc3RaiseIntentRequest';
import { ComposeUIIntentResolution } from './infrastructure/ComposeUIIntentResolution';
import { Fdc3RaiseIntentResponse } from './infrastructure/messages/Fdc3RaiseIntentResponse';
import { Fdc3FindIntentsByContextRequest } from './infrastructure/messages/Fdc3FindIntentsByContextRequest';
import { Fdc3FindIntentsByContextResponse } from './infrastructure/messages/Fdc3FindIntentsByContextResponse';
import { ComposeUIErrors } from './infrastructure/ComposeUIErrors';
import { ChannelFactory } from './infrastructure/ChannelFactory';
import { MessageRouterChannelFactory } from './infrastructure/MessageRouterChannelFactory';
import { MessageRouterIntentsClient } from './infrastructure/MessageRouterIntentsClient';
import { IntentsClient } from './infrastructure/IntentsClient';

declare global {
    interface Window {
        composeui: {
            fdc3: {
                config: AppIdentifier | undefined;
            }
        }
        fdc3: DesktopAgent;
    }
}

export class ComposeUIDesktopAgent implements DesktopAgent {
    private appChannels: Channel[] = [];
    private userChannels: Channel[] = [];
    private privateChannels: Channel[] = [];
    private currentChannel?: Channel;
    private currentChannelListeners: ComposeUIContextListener[] = [];
    private intentListeners: Listener[] = [];
    private channelFactory: ChannelFactory;
    private intentsClient: IntentsClient;

    //TODO: we should enable passing multiple channelId to the ctor.
    constructor(channelId: string, messageRouterClient: MessageRouter) {
        if (!window.composeui.fdc3.config || !window.composeui.fdc3.config.instanceId) {
            throw new Error(ComposeUIErrors.InstanceIdNotFound);
        }

        // TODO: inject this directly instead of the messageRouter
        this.channelFactory = new MessageRouterChannelFactory(messageRouterClient);
        this.intentsClient = new MessageRouterIntentsClient(messageRouterClient);


        setTimeout(
            async () => {
                await this.joinUserChannel(channelId);
                window.fdc3 = this;
                window.dispatchEvent(new Event("fdc3Ready"));
            }, 0);
    }

    //TODO
    public open(app?: string | AppIdentifier, context?: Context): Promise<AppIdentifier> {
        throw new Error("Not implemented");
    }

    public findIntent(intent: string, context?: Context, resultType?: string): Promise<AppIntent> {
        return this.intentsClient.findIntent(intent, context, resultType);
    }

    public findIntentsByContext(context: Context, resultType?: string): Promise<Array<AppIntent>> {
        return this.intentsClient.findIntentsByContext(context, resultType);
    }

    //TODO
    public findInstances(app: AppIdentifier): Promise<Array<AppIdentifier>> {
        throw new Error("Not implemented");
    }

    public broadcast(context: Context): Promise<void> {
        return new Promise((resolve, reject) => {
            if (!this.currentChannel) {
                return reject(new Error(ComposeUIErrors.CurrentChannelNotSet));
            } else {
                return resolve(this.currentChannel.broadcast(context));
            }
        });
    }

    public async raiseIntent(intent: string, context: Context, app?: string | AppIdentifier): Promise<IntentResolution> {
        return this.intentsClient.raiseIntent(intent, context, app);
    }

    //TODO
    public raiseIntentForContext(context: Context, app?: string | AppIdentifier): Promise<IntentResolution> {
        throw new Error("Not implemented");
    }

    public async addIntentListener(intent: string, handler: IntentHandler): Promise<Listener> {
        var listener = await this.channelFactory.GetIntentListener(intent, handler);

        this.intentListeners.push(listener);
        return listener;
    }

    public async addContextListener(contextType?: string | null | ContextHandler, handler?: ContextHandler): Promise<Listener> {
        if (!this.currentChannel) {
            throw new Error(ComposeUIErrors.CurrentChannelNotSet);
        }

        if (contextType && typeof contextType != 'string') {
            handler = contextType;
            contextType = null;
        }

        const listener = <ComposeUIContextListener>await this.currentChannel!.addContextListener(contextType ?? null, handler!);

        const lastContext = await this.currentChannel!.getCurrentContext(contextType ?? undefined)

        if (lastContext) {
            await listener.handleContextMessage(lastContext);
        }

        this.currentChannelListeners.push(listener);
        return listener;
    }

    public getUserChannels(): Promise<Array<Channel>> {
        return Promise.resolve(this.userChannels);
    }

    //TODO: should return AccessDenied error when a channel object is denied?
    public joinUserChannel(channelId: string): Promise<void> {
        return new Promise<void>(async (resolve, reject) => {
            if (this.currentChannel) {
                return reject(new Error(ChannelError.AccessDenied));
            }

            let channel = this.userChannels.find(innerChannel => innerChannel.id == channelId);
            if (!channel) {
                try {
                    channel = await this.channelFactory.GetUserChannel(channelId);
                    this.addChannel(channel);
                    return resolve();
                } catch (error) {
                    return reject(error);
                }
            }
            this.currentChannel = channel;
            return resolve();

        });
    }

    //TODO: should return AccessDenied error when a channel object is denied
    //TODO: should return a CreationFailed error when a channel cannot be created or retrieved (channelId failure)
    public getOrCreateChannel(channelId: string): Promise<Channel> {
        throw new Error("Not implemented.");
    }

    //TODO
    public createPrivateChannel(): Promise<PrivateChannel> {
        throw new Error("Not implemented");
    }

    public getCurrentChannel(): Promise<Channel | null> {
        return Promise.resolve(this.currentChannel ?? null);
    }

    //TODO: add messageRouter message that we are leaving the current channel to notify the backend.
    public leaveCurrentChannel(): Promise<void> {
        return new Promise<void>((resolve, reject) => {
            this.currentChannel = undefined;
            this.currentChannelListeners.forEach(listener => {
                const isUnsubscribed = listener.unsubscribe();
                if (!isUnsubscribed) {
                    return reject(new Error(`Listener couldn't unsubscribe. IsSubscribed: ${isUnsubscribed}, Listener: ${listener}`));
                }
            });
            this.currentChannelListeners = [];
            return resolve();
        });
    }

    //TODO(Lilla): we should ask the backend to give the current appMetadata back
    public getInfo(): Promise<ImplementationMetadata> {
        return new Promise<ImplementationMetadata>(async (resolve, reject) => {
            const metadata = {
                fdc3Version: "2.0",
                provider: "ComposeUI",
                providerVersion: "0.1.0-alpha.1", //TODO: version check
                optionalFeatures: {
                    OriginatingAppMetadata: false,
                    UserChannelMembershipAPIs: false
                }
            };
            resolve(<ImplementationMetadata>metadata);
        });
    }

    //TODO
    public getAppMetadata(app: AppIdentifier): Promise<AppMetadata> {
        throw new Error("Not implemented");
    }

    // Deprecated, alias to getUserChannels
    // https://fdc3.finos.org/docs/2.0/api/ref/DesktopAgent#getsystemchannels-deprecated
    public getSystemChannels(): Promise<Channel[]> {
        return this.getUserChannels();
    }

    // Deprecated, alias to joinUserChannel
    // https://fdc3.finos.org/docs/2.0/api/ref/DesktopAgent#joinchannel-deprecated
    public joinChannel(channelId: string): Promise<void> {
        return this.joinUserChannel(channelId);
    }

    private addChannel(channel: Channel): void {
        if (channel == null) return;
        switch (channel.type) {
            case "app":
                this.appChannels.push(channel);
                break;
            case "user":
                this.userChannels.push(channel);
                break;
            case "private":
                this.privateChannels.push(channel);
                break;
        }
    }
}