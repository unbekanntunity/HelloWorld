import React, { Component } from 'react';

import { ReportDialog } from '../components/Dialog';
import { DetailedPost } from '../components/Post';
import Project from '../components/Project';
import Tag from '../components/Tag';
import TopBanner from '../components/TopBanner/TopBanner';
import Discussion from '../components/Discussion';

import edit from '../images/edit.png';
import settings from '../images/settings.png';

import { sendJSONRequest } from '../requestFuncs';

import './Account.css';

class Account extends Component {
    state = {
        items: [],
        selectedIndex: 0,

        showReportDialog: false
    }

    componentDidMount() {
        this.handleChange(0);
    }

    handleChange = (index) => {
        let url = "";

        switch (index) {
            case 0:
                url = "/post/get_all"
                break;
            case 1:
                url = "/discussion/get_all"
                break;
            case 2:
                url = "/project/get_all"
                break;
        }

        sendJSONRequest("GET", url, undefined, this.props.tokens.token, {
            creatorId: this.props.user.id
        }).then(response => {
            this.setState({ items: [...response.data] })
        }, error => {
            console.log(error);
            this.props.onError(error.message);
        }).finally(() => this.setState({ selectedIndex: index }))
    }

    handleCreatorInfos = (index) => {
        console.log(this.state.items);

        let newItems = this.state.items;
        sendJSONRequest("GET", `/users/get/${newItems[index].creatorId}`, undefined, this.props.tokens.token)
            .then(response => {
                newItems[index].creatorImage = response.data.imageUrl;
                newItems[index].creatorName = response.data.userName;
                this.setState({ items: newItems })
            }, error => {
                this.props.onError(error.message);
            });
    }
     
    handleCommentCheck = (creatorId) => {
        return creatorId === this.props.user.id;
    }

    handleEditProfile = () => {

    }

    renderItems = () => {
        switch (this.state.selectedIndex) {
            case 0:
                return this.state.items.map((item, index) =>
                    <div className="account-item" key={index} >
                        <DetailedPost keyProp={index} id={item.id} creatorId={item.creatorId} creatorImage={item.creatorImage} creatorName={item.creatorName} createdAt={item.createdAt}
                            tags={item.tags} images={item.imageUrls} imageHeight={200} imageWidth={280} text={item.content} width="800px"
                            onFirstAppear={this.handleCreatorInfos} tokens={this.props.tokens} onError={this.props.onError}
                            onReportClick={() => this.setState({ showReportDialog: true })} userId={this.props.user.id} />
                    </div>
                )
            case 1:
                return this.state.items.map((item, index) =>    
                    <div className="account-item" key={index}>
                        <Discussion keyProp={index} width={600} onFirstAppear={this.handleCreatorInfos}
                            title={item.title} startMessage={item.startMessage} createdAt={item.createdAt} tags={item.tags} creatorImage={item.creatorImage}
                            lastMessage={item.lastMessage} lastMessageAuthor={item.lastMessageAuthor} lastMessageCreated={item.lastMessageCreated}
                            onFirstAppear={this.handleCreatorInfos} />
                    </div>
                )
            case 2:
                return this.state.items.map((item, index) =>
                    <div className="account-item" key={index} >
                        <Project keyProp={index} title={item.title} createdAt={item.createdAt} description={item.description} creatorId={item.creatorId}
                            images={item.images} creatorImage={item.creatorImage} tags={item.tags} width={600} imageHeight={300} imageWidth={500}
                            onReportClick={() => this.setState({ showReportDialog: true })}
                            onFirstAppear={this.handleCreatorInfos} />
                    </div>
                )
        }
    }

    render() {
        return (
            <div className="page-body">
                <div className="account-profile-section">
                    <div className="account-pic-name">
                        <img src={this.props.user.imageUrl} alt="" width={100} height={100} />
                        <p className="account-name">{this.props.user.userName}</p>
                    </div>
                    <div className="account-description-container">
                        <p className="account-description">Description</p>
                        <p>{this.props.user.description}</p>
                    </div>
                    <div className="account-tags">
                        {
                            this.props.user.tags.map((item, index) =>
                                <Tag key={index} name={item.name} fontSize="20px" margin="10px" />
                            )
                        }
                    </div>
                    <div className="account-edit-container">
                        <img src={edit} alt="" height={30} width={30} onClick={this.handleEditProfile} />
                        <img src={settings} alt="" height={30} width={30} onClick={this.props.onSettings} />
                    </div>
                </div>
                <TopBanner bgColor="#F3F2F2" onSelectionChanged={this.handleChange}>
                    <TopBanner.UnderlineItem name="Posts" selectedBorderColor="#FF9900" textColor="black"/>
                    <TopBanner.UnderlineItem name="Discussions" selectedBorderColor="#FF9900" textColor="black" />
                    <TopBanner.UnderlineItem name="Projects" selectedBorderColor="#FF9900" textColor="black" />
                    <TopBanner.UnderlineItem name="Saved" selectedBorderColor="#FF9900" textColor="black" />
                </TopBanner>
                {
                    <div className="account-items">
                    {
                        this.renderItems()
                    }
                    </div>
                }
                {
                    this.state.showReportDialog &&
                    <ReportDialog onClose={() => this.setState({ showReportDialog: false })} onNotifcation={this.props.onNotifcation} />
                }
            </div>
        )
    }
}

export default Account;